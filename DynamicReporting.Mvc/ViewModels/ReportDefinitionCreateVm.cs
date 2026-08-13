namespace DynamicReporting.Mvc.ViewModels;

public sealed class ReportDefinitionCreateVm
{
    public ReportDefinitionEditVm Report { get; set; }
        = new();

    public IReadOnlyList<TableMetadataVm> Metadata { get; set; }
        = [];
}