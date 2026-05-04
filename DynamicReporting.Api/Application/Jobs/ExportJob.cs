namespace DynamicReporting.Api.Application.Jobs;

public class ExportJob(IServiceResolver serviceResolver, IJobQueueService jobQueueService, IReportGeneratedService generatedService, IReportNotificationService notificationService) : IExportJob
{
    public async Task ExportJobAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        ServiceResolver.ExportType type, Guid reportGuid, CancellationToken cancellationToken)
    {
        var fullPath = CreateExportFile(type, reportGuid);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 2 * 1024 * 1024, useAsync: true);

        var exportService = serviceResolver.GetExportService(type);
        await exportService.ExportAsync(reportDefinitionId, filtersList, fileStream, cancellationToken);

        string fileName = $"report_{reportGuid}";

        var downloadUrl = @$"Exports\{type}\{fileName}{FileTypeNameHelper.GetFileType(type)}";

        var dto = new ReportGenerationUpdateDto()
        {
            ReportGuid = reportGuid,
            DownloadUrl = downloadUrl
        };
        await generatedService.UpdateAsync(dto);
    }

    public async Task FinalizeExportJobAsync(int jobId, Guid reportGuid)
    {
        var status = jobQueueService.GetStatusByJobId(jobId);

        if (status == nameof(HangfireJobQueueService.HangfireJobState.Succeeded))
        {
            await EntityUpdateAsync(jobId, reportGuid);
            await notificationService.NotifyReportReadyAsync(reportGuid);
        }
    }

    private async Task EntityUpdateAsync(int jobId, Guid reportGuid)
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

    private string CreateExportFile(ServiceResolver.ExportType type, Guid reportGuid)
    {
        string fileName = $"report_{reportGuid}" + FileTypeNameHelper.GetFileType(type);
        var directory = Path.Combine(Directory.GetCurrentDirectory(), @$"Exports\{type}");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }
}