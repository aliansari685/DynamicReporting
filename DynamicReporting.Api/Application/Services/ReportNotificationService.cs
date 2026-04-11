namespace DynamicReporting.Api.Application.Services;

public class ReportNotificationService(IHubContext<ReportHub> hubContext) : IReportNotificationService
{
    public async Task NotifyReportReadyAsync(Guid reportGuid, object data)
    {
        await hubContext.Clients.Group(reportGuid.ToString())
            .SendAsync("ReportReady", new
            {
                message = "گزارش شما آماده است",
                downloadUrl = data,
            });
    }
}