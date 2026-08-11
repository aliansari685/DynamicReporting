namespace DynamicReporting.Mvc.ViewModels;

public sealed class ReportDefinitionVm
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string BaseTable { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public List<SelectedColumnVm> SelectedColumns { get; set; } = [];
}