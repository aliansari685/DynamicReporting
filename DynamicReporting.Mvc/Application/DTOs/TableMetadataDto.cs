using DynamicReporting.Mvc.Application.Interfaces;

namespace DynamicReporting.Mvc.Application.DTOs;

/// <summary>
/// DTO for table metadata
/// </summary>
public class TableMetadataDto
{
    public string TableName { get; set; } = string.Empty;
    public List<ColumnMetadataDto> Columns { get; set; } = [];
}