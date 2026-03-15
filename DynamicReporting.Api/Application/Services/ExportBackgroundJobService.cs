namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService) : IExportBackgroundJobService
{
    public string ExportInBackground(int reportDefinitionId, string type, CancellationToken cancellationToken)
        => jobQueueService.Enqueue<IExportJob>(x => x.ExportJobAsync(reportDefinitionId, type, cancellationToken));
}