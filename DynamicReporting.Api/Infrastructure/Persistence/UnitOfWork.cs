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
        try
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IRepository<T>)_repositories[typeof(T)];

            var repo = new GenericRepository<T>(context);
            _repositories.Add(typeof(T), repo);
            return repo;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }
    }

    public async Task BeginTransactionAsync()
    {
        try
        {
            _transaction = await context.Database.BeginTransactionAsync();
            if (_transaction == null)
                throw new NullReferenceException("خطای داخلی");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }

    }

    public async Task CommitAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        catch (Exception ex)
        {
            await _transaction!.RollbackAsync();
            Log.Error(ex.Message);
            throw;
        }
        finally
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
    }
}