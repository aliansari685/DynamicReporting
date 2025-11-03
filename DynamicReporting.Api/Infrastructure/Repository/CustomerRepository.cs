namespace DynamicReporting.Api.Infrastructure.Repository;

public class CustomerRepository(ShopTestDbContext context) : IRepository<Customer>
{
    public void Add(Customer entity) => context.AddAsync(entity);

    public void Update(Customer entity)
    {
        throw new NotImplementedException();
    }

    public void Remove(Customer entity)
    {
        throw new NotImplementedException();
    }

    public Customer GetById(int id)
    {
        throw new NotImplementedException();
    }

    public List<Customer> GetAllToList()
    {
        throw new NotImplementedException();
    }

    public IQueryable<Customer> GetAll()
    {
        throw new NotImplementedException();
    }

    public Customer GetByProperty(Expression<Func<Customer, bool>> predicate)
    {
        throw new NotImplementedException();
    }
}