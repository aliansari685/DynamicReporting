namespace DynamicReporting.Api.Domain.Models;

/// <summary>
/// مدل جهت معرفی و نمایش متادیتا جدول  یا ستون
/// </summary>
public class DisplayMetadata
{
    /// <summary>
    /// نام فیزیکی مثل customers
    /// </summary>
    public required string PhysicalName { get; set; }

    /// <summary>
    ///نام نمایشی مثل مشتریان
    /// </summary>
    public string? DisplayName { get; set; }
}