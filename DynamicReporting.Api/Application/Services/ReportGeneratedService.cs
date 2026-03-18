namespace DynamicReporting.Api.Application.Services;

public class ReportGeneratedService(IUnitOfWork uow) : IReportGeneratedService
{
    public async Task<ReportGeneration> GetByGuidAsync(Guid id) =>
        await uow.Repository<ReportGeneration>().GetByPropertyAsync(x => x.ReportGuid == id) ?? throw new NullReferenceException("شناسه وجود ندارد");

    public IEnumerable<ReportGeneration> GetAll() => uow.Repository<ReportGeneration>().GetAll();

    public async Task<List<ReportGeneration>> GetAllToListAsync() => await uow.Repository<ReportGeneration>().GetAllToListAsync();

    public async Task<ReportGeneration?> GetByPropertyAsync(Expression<Func<ReportGeneration, bool>> predicate) => await uow.Repository<ReportGeneration>().GetByPropertyAsync(predicate);

    public async Task CreateAsync(ReportGenerationDto dto)
    {
        await uow.BeginTransactionAsync();
        var reportGeneration = dto.Adapt<ReportGeneration>();
        uow.Repository<ReportGeneration>().Add([reportGeneration]);
        await uow.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await uow.BeginTransactionAsync();
        var repo = uow.Repository<ReportGeneration>();
        var entity = await repo.GetByPropertyAsync(x => x.ReportGuid == id) ?? throw new NullReferenceException("شناسه وجود ندارد");
        uow.Repository<ReportGeneration>().Remove([entity]);
        await uow.CommitAsync();
    }
}