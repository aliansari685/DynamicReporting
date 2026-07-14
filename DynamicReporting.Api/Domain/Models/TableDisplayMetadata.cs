namespace DynamicReporting.Api.Domain.Models;

/// <summary>
/// مدل نمایش جدول به همراه نمایش فارسی و ستون های آن
/// </summary>
public class TableDisplayMetadata
{
    /// <summary>
    /// نام فیزیکی جدول
    /// </summary>
    public required string TableName { get; set; }

    /// <summary>
    /// نام فارسی جدول
    /// </summary>
    public required string? TableDisplayName { get; set; }

    /// <summary>
    /// نمایش ستون های آن
    /// </summary>
    public required List<DisplayColumnsMetadata> Columns { get; set; } = [];
}