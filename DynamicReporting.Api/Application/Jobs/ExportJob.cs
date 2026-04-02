namespace DynamicReporting.Api.Application.Jobs;

public class ExportJob(IReportExportServiceResolver serviceResolver, IJobQueueService jobQueueService, IReportGeneratedService generatedService) : IExportJob
{
    public async Task ExportJobAsync(int reportDefinitionId, string type, Guid reportGuid, CancellationToken cancellationToken = default)
    {
        var fullPath = CreateExportFile(type, reportGuid);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 2 * 1024 * 1024, useAsync: true);

        var exportService = serviceResolver.GetService(type);
        await exportService.ExportAsync(reportDefinitionId, fileStream, cancellationToken);

        var dto = new ReportGenerationUpdateDto()
        {
            ReportGuid = reportGuid,
            DownloadUrl = fullPath
        };
        await generatedService.UpdateAsync(dto);
    }

    public async Task FinalizeExportJobAsync(int jobId, Guid reportGuid)
    {
        var status = jobQueueService.GetStatusByJobId(jobId);

        if (status == nameof(HangfireJobQueueService.HangfireJobState.Succeeded))
        {
            var expDateTime = jobQueueService.GetExpireDateTimeByJobId(jobId);

            var dto = new ReportGenerationUpdateDto
            {
                ReportGuid = reportGuid,
                JobId = jobId,
                ExpDateTime = expDateTime
            };
            await generatedService.UpdateAsync(dto);
        }
    }

    private string CreateExportFile(string type, Guid reportGuid)
    {
        string fileName = $"report_{reportGuid}";

        fileName += type.ToLower() switch
        {
            "excel" => ".xlsx",
            "pdf" => ".pdf",
            _ => throw new ArgumentOutOfRangeException(type, "ورودی فایل وجود ندارد")
        };

        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }
}