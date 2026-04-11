namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportNotificationService
{
    public Task NotifyReportReadyAsync(Guid reportGuid, object data);
}