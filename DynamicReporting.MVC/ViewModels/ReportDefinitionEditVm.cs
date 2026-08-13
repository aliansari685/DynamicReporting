namespace DynamicReporting.Mvc.ViewModels;

public sealed class ReportDefinitionEditVm
{
    public string Name { get; set; } = string.Empty;

    public string BaseTable { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public List<SelectedColumnVm> SelectedColumns { get; set; } = [];
}