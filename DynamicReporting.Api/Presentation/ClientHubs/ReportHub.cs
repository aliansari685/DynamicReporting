namespace DynamicReporting.Api.Presentation.ClientHubs;

public class ReportHub(
    IReportGeneratedService generatedService) : Hub
{
    public async Task<string> Test()
    {
        await Clients.Caller.SendAsync("Test");

        return "این پاسخ از سمت سرور و متد تست است";
    }


    public async Task JoinGroup(string reportGuid)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            reportGuid);


        if (!Guid.TryParse(reportGuid, out var guid))
            return;


        try
        {
            var report =
                await generatedService.GetByGuidAsync(
                    guid,
                    CancellationToken.None);


            if (string.Equals(
                    report.Status,
                    nameof(HangfireJobState.Succeeded),
                    StringComparison.CurrentCultureIgnoreCase))
            {
                await Clients.Caller.SendAsync(
                    "ReportReady",
                    new
                    {
                        reportGuid = guid,
                        message =
                            "گزارش شما آماده است؛ برای دانلود به صفحه گزارشات مراجعه کنید."
                    });
            }
        }
        catch (KeyNotFoundException)
        {
            // هنوز رکورد گزارش ساخته نشده است.
            // Notification از طریق Job بعداً ارسال خواهد شد.
        }
    }


    public async Task LeaveGroup(string reportGuid)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            reportGuid);
    }
}