namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService, IReportGeneratedService generatedService) : IExportBackgroundJobService
{
    public async Task<Guid> ExportInBackground(int reportDefinitionId, string type, CancellationToken cancellationToken)
    {
        var reportGuid = Guid.NewGuid();
        int exportInBackgroundJobId = 0;
        try
        {
            var jobIdString = jobQueueService.Enqueue<IExportJob>(x => x.ExportJobAsync(reportDefinitionId, type, reportGuid, cancellationToken));

            exportInBackgroundJobId = int.Parse(jobIdString);

            var generation = new ReportGenerationRequestDto
            {
                ReportGuid = reportGuid,
                JobId = exportInBackgroundJobId
            };

            await generatedService.CreateAsync(generation);

            jobQueueService.ContinueJob<IExportJob>(exportInBackgroundJobId, x => x.FinalizeExportJobAsync(exportInBackgroundJobId, reportGuid));

            return reportGuid;

            //todo: آپدیت لینک دانلود پس از تکمیل

        }
        catch (Exception ex)
        {
            jobQueueService.Delete(exportInBackgroundJobId);
            throw new OperationCanceledException("عملیات با شکست مواجه شد", ex);
        }

        //todo:  هروقت اماده شد لینک دانلود بزارم - نوتیف بده - 

    }
}