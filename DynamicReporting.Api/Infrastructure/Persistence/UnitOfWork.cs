namespace DynamicReporting.Api.Infrastructure.Persistence;

public class UnitOfWork(ShopTestDbContext context) : IUnitOfWork
{
    /// <summary>
    /// کش جنریک ریپازیتوری ها برای استفاده دوباره
    /// </summary>
    private readonly Dictionary<Type, object> _repositories = new();

    /// <summary>
    /// تراکنش فعلی ک مقدار دهی میشود در متد آغاز تراکنش
    /// </summary>
    private IDbContextTransaction? _transaction;

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
            return (IRepository<T>)_repositories[typeof(T)];

        var repo = new GenericRepository<T>(context);
        _repositories.Add(typeof(T), repo);
        return repo;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
        if (_transaction == null)
            throw new NullReferenceException("خطای داخلی");
    }

    public async Task CommitAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        catch
        {
            await _transaction!.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
    }
}