namespace DynamicReporting.Api.Infrastructure.Persistence.Repository;

public class GenericRepository<T>(ShopTestDbContext shopTestDbContext) : IRepository<T> where T : class
{
    public readonly DbSet<T> DbSet = shopTestDbContext.Set<T>();
    public void Add(List<T> entity) => DbSet.AddRange(entity);

    public void Update(List<T> entity) => DbSet.UpdateRange(entity);

    public void Remove(List<T> entity) => DbSet.RemoveRange(entity);

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public IQueryable<T> GetAll() => DbSet.AsNoTracking();

    public async Task<List<T>> GetAllToListAsync() => await DbSet.ToListAsync();
 
    public async Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate) => await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
}