namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService, IReportGeneratedService generatedService) : IExportBackgroundJobService
{
    public async Task<Guid> ExportInBackground(int reportDefinitionId, List<FilterCondition>? filtersList, string sortColumn,
        ServiceResolver.ExportType type, CancellationToken cancellationToken)
    {
        var reportGuid = Guid.NewGuid();
        int exportInBackgroundJobId = 0;
        try
        {
            var jobIdString = jobQueueService.Enqueue<IExportJob>(x => x.ExportJobAsync(reportDefinitionId, filtersList, sortColumn, type, reportGuid, cancellationToken));

            exportInBackgroundJobId = int.Parse(jobIdString);

            var generation = new ReportGenerationRequestDto
            {
                ReportGuid = reportGuid,
                JobId = exportInBackgroundJobId,
                FileType = type.ToString()
            };

            await generatedService.CreateAsync(generation);

            jobQueueService.ContinueJob<IExportJob>(exportInBackgroundJobId, x => x.FinalizeExportJobAsync(exportInBackgroundJobId, reportGuid));

            return reportGuid;
        }
        catch (Exception ex)
        {
            jobQueueService.Delete(exportInBackgroundJobId);
            throw new OperationCanceledException("عملیات با شکست مواجه شد", ex);
        }
    }
}