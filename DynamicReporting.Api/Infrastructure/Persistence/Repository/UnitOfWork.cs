namespace DynamicReporting.Api.Infrastructure.Persistence.Repository;

public class UnitOfWork(ShopTestDbContext shopTestDbContext) : IUnitOfWork
{
    public ShopTestDbContext DbContext => shopTestDbContext;
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

            var repo = new GenericRepository<T>(shopTestDbContext);
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
            _transaction = await shopTestDbContext.Database.BeginTransactionAsync();
            if (_transaction == null)
                throw new NullReferenceException("خطای داخلی");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }

    }

    public async Task<bool> CommitAsync()
    {
        try
        {
            await shopTestDbContext.SaveChangesAsync();
            await _transaction!.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _transaction!.RollbackAsync();
            Log.Error(ex, "خطا در ثبت ردیف");
            throw;
        }
        finally
        {
            if (_transaction != null) await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}