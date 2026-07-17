namespace DynamicReporting.Api.Domain.Models;

/// <summary>
/// مدل نمایش ستون های قابل مرتب سازی جهت نمایش و انتخاب
/// </summary>
public class SortableColumn
{
    /// <summary>
    /// نام جدول
    /// </summary>
    public required string Table { get; set; }

    /// <summary>
    /// نام ستون
    /// </summary>
    public required string Column { get; set; }

    /// <summary>
    /// نام کامل ستون با فرمت Table.Column (برای استفاده در SortBy)
    /// </summary>
    public required string Field { get; set; }

    /// <summary>
    /// نام فارسی جدول
    /// </summary>
    public required string DisplayName { get; set; }
}