namespace DynamicReporting.Api.Domain.Models;

/// <summary>
///     این مدل برای اعمال فیلترها و شرط های است
/// </summary>
public class FilterOperatorInfo
{
    /// <summary>
    ///     نوع عملیات مثل eq
    /// </summary>
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    ///     نمایش فارسی عملیات مثل مقایسه یا برابری
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}