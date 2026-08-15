namespace DynamicReporting.Api.Domain.Models;

/// <summary>
///     نمایش مدل جدول
/// </summary>
public class TableMetadata
{
    /// <summary>
    ///     نام فیزیکی جدول
    /// </summary>
    public required string TableName { get; set; }

    /// <summary>
    ///     نام نمایشی (فارسی) جدول
    /// </summary>
    public required string? DisplayName { get; set; }

    /// <summary>
    ///     لیست ستون ها
    /// </summary>
    public required List<DisplayMetadata> Columns { get; set; } = [];
}