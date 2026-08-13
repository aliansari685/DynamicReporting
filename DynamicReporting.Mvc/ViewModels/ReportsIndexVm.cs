namespace DynamicReporting.Mvc.ViewModels;

public sealed class ReportsIndexVm
{
    public IReadOnlyList<ReportDefinitionVm> Reports { get; init; } = [];

    public ReportDefinitionVm? DefaultReport { get; init; }
    public ReportDefinitionVm? SelectedReport { get; set; }
}