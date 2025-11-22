namespace DynamicReporting.Api.Domain.Models;

[NotMapped]
public class SelectedColumn
{
    [MaxLength(255)] public required string Column { get; set; }
    [MaxLength(255)] public required string Table { get; set; }
}