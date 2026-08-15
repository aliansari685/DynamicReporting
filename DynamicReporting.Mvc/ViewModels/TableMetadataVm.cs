namespace DynamicReporting.Mvc.ViewModels;

/// <summary>
/// DTO for table Metadata
/// </summary>
public class TableMetadataVm
{
    public string TableName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public List<ColumnMetadataVm> Columns { get; set; } = [];
}