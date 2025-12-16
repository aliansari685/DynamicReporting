namespace DynamicReporting.Api.Application.Services;

public class ReportDefinitionService(IUnitOfWork uow, IBaseTableResolver baseTableResolver) : IReportDefinitionService
{
    public async Task<ReportDefinition> GetByIdAsync(int id) =>
        await uow.Repository<ReportDefinition>().GetByIdAsync(id) ?? throw new NullReferenceException("شناسه وجود ندارد");

    public IEnumerable<ReportDefinition> GetAll() => uow.Repository<ReportDefinition>().GetAll();

    public async Task<List<ReportDefinition>> GetAllToListAsync() => await uow.Repository<ReportDefinition>().GetAllToListAsync();

    public async Task<ReportDefinition?> GetByPropertyAsync(Expression<Func<ReportDefinition, bool>> predicate) => await uow.Repository<ReportDefinition>().GetByPropertyAsync(predicate);

    public async Task CreateAsync(ReportDefinitionDto dto)
    {
        await uow.BeginTransactionAsync();

        var reportDefinition = dto.Adapt<ReportDefinition>();

        // پیدا کردن بیس تیبل
        reportDefinition.BaseTable =
            baseTableResolver.Resolve(dto.SelectedColumns);

        uow.Repository<ReportDefinition>().Add([reportDefinition]);

        await uow.CommitAsync();
    }
    public async Task UpdateAsync(int id, ReportDefinitionDto definition)
    {
        await uow.BeginTransactionAsync();
        var mainReportDefinition = definition.Adapt<ReportDefinition>();
        mainReportDefinition.Id = id;
        mainReportDefinition.UpdatedAt = DateTime.UtcNow;
        uow.Repository<ReportDefinition>().Update([mainReportDefinition]);
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

        var all = await repo.GetAllToListAsync();
        all.ToList().ForEach(x => x.IsDefault = false);

        var item = all.FirstOrDefault(x => x.Id == id)
                   ?? throw new NullReferenceException("شناسه وجود ندارد");

        item.IsDefault = true;

        await uow.CommitAsync();
    }
}