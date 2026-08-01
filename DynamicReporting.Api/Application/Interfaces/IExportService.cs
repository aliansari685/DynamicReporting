namespace DynamicReporting.Api.Application.Interfaces;

/// <summary>
///     قراردادی مشترک برای خروجی گرفتن داده
/// </summary>
public interface IExportService
{
    /// <summary>
    ///     متد اصلی خروجی گرفتن
    ///     برای خروجی اکسل از پکیج SpreadCheetah جهت بهینه‌سازی مصرف رم
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش </param>
    /// <param name="filtersList"></param>
    /// <param name="outputStream">محل ساخت خروجی دیتا مثل رم یا هارد</param>
    /// <param name="sortColumn">مرتب سازی بر اساس فلان ستون</param>
    /// <param name="cancellationToken">نوع کنسل کردن درخواست توسط کاربر</param>
    /// <returns></returns>
    Task<bool> ExportAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream,
        SortableColumnDto sortColumn, CancellationToken cancellationToken = default);
}