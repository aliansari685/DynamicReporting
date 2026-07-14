namespace DynamicReporting.Api.Domain.Models;

public class DisplayColumnsMetadata
{
    /// <summary>
    /// نام فیزیکی مثل customers
    /// </summary>
    public required string PhysicalName { get; set; }

    /// <summary>
    ///نام نمایشی مثل مشتریان
    /// </summary>
    public required string? DisplayName { get; set; }

    /// <summary>
    /// نمایش فیلتر های قابل اعمال
    /// </summary>
    public required List<FilterOperatorInfo> SupportedOperators { get; set; }
}