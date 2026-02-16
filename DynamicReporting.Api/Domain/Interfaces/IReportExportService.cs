namespace DynamicReporting.Api.Domain.Interfaces;

/// <summary>
/// قراردادی مشترک برای خروجی گرفتن داده
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// خروجی گرفتن اکسل با پرفورمنس بالا با که برای داده‌های حجیم بهینه شده است.
    /// کمتری مصرف رم و پکیج SpreadCheetah
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش </param>
    /// <param name="outputStream">نوع خروجی دیتا مثل رم یا هارد</param>
    /// <param name="cancellationToken">کنسل کردن درخواست توسط کاربر</param>
    /// <returns></returns>
    Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// خروجی گرفتن سریع اکسل با پرفورمنس معمولی با که برای داده‌های متوسط
    /// نهایت مصرف رم و پکیج epPlus
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش </param>
    /// <param name="outputStream">نوع خروجی دیتا مثل رم یا هارد</param>
    /// <param name="cancellationToken">کنسل کردن درخواست توسط کاربر</param>
    /// <returns></returns>
    Task ExportToExcelFastAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default);

}