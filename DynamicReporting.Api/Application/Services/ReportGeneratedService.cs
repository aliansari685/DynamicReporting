namespace DynamicReporting.Api.Application.Services;

public class ReportGeneratedService(IUnitOfWork uow, IJobQueueService jobQueueService) : IReportGeneratedService
{
    public async Task<ReportGenerationResponseDto> GetByGuidAsync(Guid id)
    {
        // var result = await uow.Repository<ReportGeneration>().GetByPropertyAsync(x => x.ReportGuid == id) ??
        var result = await GetByPropertyAsync(x => x.ReportGuid == id) ??
                     throw new NullReferenceException("شناسه وجود ندارد");

        var resultMapping = result.Adapt<ReportGenerationResponseDto>();
        resultMapping.Status = jobQueueService.GetStatusByJobId(resultMapping.JobId);
        return resultMapping;
    }

    public async Task<List<ReportGenerationResponseDto>> GetAllToListAsync()
    {
        var results = await uow.Repository<ReportGeneration>()
            .GetAllToListAsync();

        var resultMappings = results.Adapt<List<ReportGenerationResponseDto>>();

        foreach (var item in resultMappings)
        {
            var originalReport = results.FirstOrDefault(r => r.ReportGuid == item.ReportGuid) ??
                                 throw new MissingMemberException("خطای داخلی در یک ردیف ");
            item.Status = jobQueueService.GetStatusByJobId(originalReport.JobId);
        }
        return resultMappings;
    }

    public async Task<string> GetStatusByGuid(Guid id)
    {
        //  var result = await uow.Repository<ReportGeneration>().GetByPropertyAsync(x => x.ReportGuid == id) ??
        var result = await GetByPropertyAsync(x => x.ReportGuid == id) ??
                     throw new NullReferenceException("شناسه وجود ندارد");

        return jobQueueService.GetStatusByJobId(result.JobId).HangfireStateToPersian();
    }

    public async Task<ReportGeneration?> GetByPropertyAsync(Expression<Func<ReportGeneration, bool>> predicate)
    {
        return await uow.Repository<ReportGeneration>().GetByPropertyAsync(predicate);
    }

    public async Task<bool> CreateAsync(ReportGenerationRequestDto dto)
    {
        await uow.BeginTransactionAsync();
        var reportGeneration = dto.Adapt<ReportGeneration>();
        uow.Repository<ReportGeneration>().Add([reportGeneration]);
        return await uow.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await uow.BeginTransactionAsync();
        var entity = await GetByPropertyAsync(x => x.ReportGuid == id) ??
                     throw new NullReferenceException("شناسه وجود ندارد");
        uow.Repository<ReportGeneration>().Remove([entity]);
        await uow.CommitAsync();
    }
}