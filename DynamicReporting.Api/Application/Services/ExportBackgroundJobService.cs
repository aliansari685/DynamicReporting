namespace DynamicReporting.Api.Application.Services;

public class ExportBackgroundJobService(IJobQueueService jobQueueService, IReportGeneratedService generatedService) : IExportBackgroundJobService
{
    public string ExportInBackground(int reportDefinitionId, string type, CancellationToken cancellationToken)
    {
        string res = jobQueueService.Enqueue<IExportJob>(x => x.ExportJobAsync(reportDefinitionId, type, cancellationToken));

        Log.Information("Id:" + res);

        //var dto = new ReportGenerationDto
        //{
        //    ReportGuid = new Guid(res)
        //};

        //    generatedService.CreateAsync(new ReportGenerationDto());

        //todo:  هروقت اماده شد لینک دانلود بزارم - جاب جدید بزارم زمان انقضاش رسید فایلو حذف کنه -نوتیف بده - 

        return res;
    }
}