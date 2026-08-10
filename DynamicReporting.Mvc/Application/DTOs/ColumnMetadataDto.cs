namespace DynamicReporting.Mvc.Application.Interfaces;

/// <summary>
/// DTO for column metadata
/// </summary>
public class ColumnMetadataDto
{
    public string PhysicalName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}