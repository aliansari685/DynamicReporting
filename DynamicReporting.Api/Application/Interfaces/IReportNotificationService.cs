namespace DynamicReporting.Api.Application.Interfaces;

/// <summary>
/// رابط بین برنامه و سرویس سیگنال آرم برای نوتیف دادن
/// </summary>
public interface IReportNotificationService
{
    /// <summary>
    /// متد نمایش محتوای نوتیف  
    /// </summary>
    /// <param name="reportGuid">شناسه گزارش یا شناسه گروه</param>
    /// <param name="data">داده ی قابل نمایش بصورت جیسون</param>
    /// <returns></returns>
    public Task NotifyReportReadyAsync(Guid reportGuid, object data);
}