namespace DynamicReporting.Api.Application.Services;

public class ReportNotificationService(IHubContext<ReportHub> hubContext) : IReportNotificationService
{
    public async Task NotifyReportReadyAsync(Guid reportGuid)
    {
        await hubContext.Clients.Group(reportGuid.ToString() /*data or url link or everything*/)
            .SendAsync("ReportReady", new
            {
                message = "گزارش شما آماده است برای دانلود به صفحه ی گزارشات مراجعه کنید"
            });
    }
}