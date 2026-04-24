namespace DynamicReporting.Api.Presentation.ClientHubs;

public class ReportHub : Hub
{
    public async Task<string> Test()
    {
        await Clients.Caller.SendAsync("Test");
        return "این پاسخ از سمت سرور و متد تست است";
    }

    public async Task JoinGroup(string reportGuid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, reportGuid);
    }

    public async Task LeaveGroup(string reportGuid)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, reportGuid);
    }
}