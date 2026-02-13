namespace DynamicReporting.Api.Domain.Interfaces;

/// <summary>
/// قراردادی مشترک برای خروجی گرفتن داده
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// خروجی گرفتن از نوع اکسل
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش </param>
    /// <param name="outputStream">نوع خروجی دیتا مثل رم یا هارد</param>
    /// <param name="cancellationToken">کنسل کردن درخواست توسط کاربر</param>
    /// <returns></returns>
    Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default);
}