namespace DynamicReporting.Api.Domain.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;

    Task<int> SaveChangesAsync();

    void BeginTransaction();
    Task CommitAsync();
    Task RollbackAsync();
}