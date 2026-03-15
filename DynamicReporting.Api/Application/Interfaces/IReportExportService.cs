namespace DynamicReporting.Api.Application.Interfaces;

/// <summary>
/// قراردادی مشترک برای خروجی گرفتن داده
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// متد اصلی خروجی گرفتن 
    ///برای خروجی اکسل از پکیج SpreadCheetah جهت بهینه‌سازی مصرف رم 
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش </param>
    /// <param name="outputStream">محل ساخت خروجی دیتا مثل رم یا هارد</param>
    /// <param name="cancellationToken">نوع کنسل کردن درخواست توسط کاربر</param>
    /// <returns></returns>
    Task ExportAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default);
}