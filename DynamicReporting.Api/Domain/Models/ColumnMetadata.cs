namespace DynamicReporting.Api.Domain.Models;

public class ColumnMetadata
{
    public required string ColumnName { get; set; }
    public string? Title { get; set; }
}