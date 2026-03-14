namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService) : IExportBackgroundJobService
{
    public string ExportToExcelInBackground(int reportDefinitionId, CancellationToken cancellationToken)
    {
        return jobQueueService.Enqueue<IExportJob>(x => x.ExportToExcelJobAsync(reportDefinitionId, cancellationToken));
    }
}