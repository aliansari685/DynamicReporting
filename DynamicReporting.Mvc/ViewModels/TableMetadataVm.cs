namespace DynamicReporting.Mvc.ViewModels;

/// <summary>
/// DTO for table Metadata
/// </summary>
public class TableMetadataVm
{
    public string TableName { get; set; } = string.Empty;
    public List<ColumnMetadataVm> Columns { get; set; } = [];
}