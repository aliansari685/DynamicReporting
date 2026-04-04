namespace DynamicReporting.Api.Application.Services;

public class ReportNotificationService(IHubContext<ReportHub> hubContext, ReportGeneratedService generatedService) : IReportNotificationService
{
    //todo: using and test with console - تقسیم کن اگر تونستی
    public async Task NotifyReportReadyAsync(Guid reportGuid)
    {
        var result = await generatedService.GetByGuidAsync(reportGuid);
        var downloadUrl = result.DownloadUrl ?? string.Empty;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), downloadUrl);

        await hubContext.Clients.Group(reportGuid.ToString())
            .SendAsync("ReportReady", new
            {
                message = "گزارش شما آماده است",
                downloadUrl = fullPath,
            });
    }
}