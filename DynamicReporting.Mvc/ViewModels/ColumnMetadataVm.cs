namespace DynamicReporting.Mvc.ViewModels;

/// <summary>
/// DTO for column Metadata
/// </summary>
public class ColumnMetadataVm
{
    public string PhysicalName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}