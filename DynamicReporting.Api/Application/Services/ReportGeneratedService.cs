namespace DynamicReporting.Api.Application.Services;

public class ReportGeneratedService(IUnitOfWork uow, IJobQueueService jobQueueService) : IReportGeneratedService
{
    public async Task<ReportGenerationResponseDto> GetByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await GetByPropertyAsync(x => x.ReportGuid == id, cancellationToken) ??
                     throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");

        var resultMapping = result.Adapt<ReportGenerationResponseDto>();
        resultMapping.Status = jobQueueService.GetStatusByJobId(resultMapping.JobId);
        return resultMapping;
    }

    public async Task<List<ReportGenerationResponseDto>> GetAllToListAsync(CancellationToken cancellationToken)
    {
        var reports = await uow.Repository<ReportGeneration>()
            .GetAllToListAsync(cancellationToken);

        reports = reports
            .OrderByDescending(x => x.CreateAt)
            .ToList();

        var resultMappings =
            reports.Adapt<List<ReportGenerationResponseDto>>();

        for (var i = 0; i < resultMappings.Count; i++)
        {
            resultMappings[i].Status =
                jobQueueService.GetStatusByJobId(
                    reports[i].JobId).HangfireStateToPersian();
        }

        return resultMappings;
    }

    public async Task<string> GetStatusPersianByGuid(Guid id, CancellationToken cancellationToken)
    {
        var result = await GetByPropertyAsync(x => x.ReportGuid == id, cancellationToken) ??
                     throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");

        return jobQueueService.GetStatusByJobId(result.JobId).HangfireStateToPersian();
    }

    public async Task<string> GetStatusByGuid(Guid id, CancellationToken cancellationToken)
    {
        var result = await GetByPropertyAsync(x => x.ReportGuid == id, cancellationToken) ??
                     throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");

        return jobQueueService.GetStatusByJobId(result.JobId);
    }

    public async Task<bool> CreateAsync(ReportGenerationRequestDto dto)
    {
        await uow.BeginTransactionAsync();
        var reportGeneration = dto.Adapt<ReportGeneration>();
        uow.Repository<ReportGeneration>().Add([reportGeneration]);
        return await uow.CommitAsync();
    }

    public async Task<bool> UpdateAsync(ReportGenerationUpdateDto dto, CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync();

        var result = await GetByPropertyAsync(x => x.ReportGuid == dto.ReportGuid, cancellationToken) ??
                     throw new KeyNotFoundException($"گزارش با شناسه {dto.ReportGuid} یافت نشد.");

        result.DownloadUrl = dto.DownloadUrl ?? result.DownloadUrl;
        result.ExpDateTime = dto.ExpDateTime ?? result.ExpDateTime;

        uow.Repository<ReportGeneration>().Update([result]);
        return await uow.CommitAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync();
        var entity = await GetByPropertyAsync(x => x.ReportGuid == id, cancellationToken) ??
                     throw new KeyNotFoundException($"گزارش با شناسه {id} یافت نشد.");
        uow.Repository<ReportGeneration>().Remove([entity]);
        await uow.CommitAsync();
    }

    public async Task<ReportGeneration?> GetByPropertyAsync(Expression<Func<ReportGeneration, bool>> predicate, CancellationToken cancellationToken)
    {
        return await uow.Repository<ReportGeneration>().GetByPropertyAsync(predicate, cancellationToken);
    }
}