namespace DynamicReporting.Api.Domain.Models;

public class TableMetadata
{
    public required string TableName { get; set; }
    public required List<string> Columns { get; set; } = [];
}