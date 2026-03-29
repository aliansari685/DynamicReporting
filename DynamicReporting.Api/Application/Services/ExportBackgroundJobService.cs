namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService, IReportGeneratedService generatedService) : IExportBackgroundJobService
{
    public async Task<string> ExportInBackground(int reportDefinitionId, string type, CancellationToken cancellationToken)
    {
        string resultId = "";
        try
        {
            resultId = jobQueueService.Enqueue<IExportJob>(x => x.ExportJobAsync(reportDefinitionId, type, cancellationToken));

            var generation = new ReportGenerationRequestDto
            {
                JobId = int.Parse(resultId)
            };

            await generatedService.CreateAsync(generation);

            return resultId;
        }
        catch (Exception ex)
        {
            jobQueueService.Delete(resultId);
            throw new OperationCanceledException("عملیات با شکست مواجه شد", ex);
        }

        //todo:  هروقت اماده شد لینک دانلود بزارم - جاب جدید بزارم زمان انقضاش رسید فایلو حذف کنه -نوتیف بده - 

    }
}