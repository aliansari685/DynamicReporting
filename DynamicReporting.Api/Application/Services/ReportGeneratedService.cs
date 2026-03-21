namespace DynamicReporting.Api.Application.Services;

public class ReportGeneratedService(IUnitOfWork uow) : IReportGeneratedService
{
    public async Task<ReportGenerationResponseDto> GetByGuidAsync(Guid id)
    {
        var result = await uow.Repository<ReportGeneration>().GetByPropertyAsync(x => x.ReportGuid == id) ??
                     throw new NullReferenceException("شناسه وجود ندارد");
        var resultMapping = result.Adapt<ReportGenerationResponseDto>();
        resultMapping.Status = GetStatus(id);
        return resultMapping;
    }

    public async Task<List<ReportGenerationResponseDto>> GetAllToListAsync()
    {
        var results = await uow.Repository<ReportGeneration>()
            .GetAllToListAsync();

        var resultMappings = results.Adapt<List<ReportGenerationResponseDto>>();

        foreach (var item in resultMappings)
        {
            var originalReport = results.FirstOrDefault(r => r.ReportGuid == item.ReportGuid) ?? throw new MissingMemberException("خطای داخلی در یک ردیف ");
            item.Status = GetStatus(originalReport.ReportGuid);
        }
        return resultMappings;
    }

    public async Task<ReportGeneration?> GetByPropertyAsync(Expression<Func<ReportGeneration, bool>> predicate) => await uow.Repository<ReportGeneration>().GetByPropertyAsync(predicate);

    public string GetStatus(Guid id)
    {
        using var connection = JobStorage.Current.GetConnection();


        //   GuidConverter converter = new GuidConverter();
        //  var idString = converter.ConvertToString(id);
        // var job = connection.GetJobData(idString));


        var job = connection.GetJobData(id.ToString());
        var state = connection.GetStateData(job.State);

        return state.Name.HangfireStateToPersian();
    }

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