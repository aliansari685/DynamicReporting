namespace DynamicReporting.Api.Application.Services;

public class ReportNotificationService(IHubContext<ReportHub> hubContext) : IReportNotificationService
{
    public async Task NotifyReportReadyAsync(Guid reportGuid)
    {
        await hubContext.Clients
            .Group(reportGuid.ToString())
            .SendAsync("ReportReady", new
            {
                reportGuid,
                message = "گزارش شما آماده است برای دانلود به صفحه گزارشات مراجعه کنید."
            });
    }
}