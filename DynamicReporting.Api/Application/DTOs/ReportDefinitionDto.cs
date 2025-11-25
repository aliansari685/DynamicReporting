namespace DynamicReporting.Api.Application.DTOs;

public class ReportDefinitionDto
{
    public string Name { get; set; } = null!;
    public string BaseTable { get; set; } = null!;
    public List<SelectedColumn> SelectedColumns { get; set; } = [];
    public string? CreatedBy { get; set; }
    public bool IsDefault { get; set; }
}