namespace DynamicReporting.Api.Application.Services;

public class ReportDefinitionService(IUnitOfWork uow) : IReportDefinitionService
{
    public async Task<ReportDefinition> GetByIdAsync(int id) =>
        await uow.Repository<ReportDefinition>().GetByIdAsync(id) ?? throw new NullReferenceException("شناسه وجود ندارد");

    public async Task<IEnumerable<ReportDefinition>> GetAllAsync() => await uow.Repository<ReportDefinition>().GetAllAsync();
    public async Task<ReportDefinition?> GetByPropertyAsync(Expression<Func<ReportDefinition, bool>> predicate) => await uow.Repository<ReportDefinition>().GetByPropertyAsync(predicate);

    public async Task CreateAsync(List<ReportDefinition> entity)
    {
        await uow.BeginTransactionAsync();
        uow.Repository<ReportDefinition>().Add(entity);
        await uow.CommitAsync();
    }

    public async Task UpdateAsync(List<ReportDefinition> entity)
    {
        await uow.BeginTransactionAsync();
        uow.Repository<ReportDefinition>().Update(entity);
        await uow.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await uow.BeginTransactionAsync();

        var repo = uow.Repository<ReportDefinition>();
        var entity = await repo.GetByIdAsync(id) ?? throw new NullReferenceException("شناسه وجود ندارد");
        uow.Repository<ReportDefinition>().Remove([entity]);
        await uow.CommitAsync();
    }

    public async Task SetDefaultAsync(int id)
    {
        await uow.BeginTransactionAsync();

        var repo = uow.Repository<ReportDefinition>();

        var all = await repo.GetAllAsync();
        all.ToList().ForEach(x => x.IsDefault = false);

        var item = all.FirstOrDefault(x => x.Id == id)
                   ?? throw new NullReferenceException("شناسه وجود ندارد");

        item.IsDefault = true;

        await uow.CommitAsync();
    }
}