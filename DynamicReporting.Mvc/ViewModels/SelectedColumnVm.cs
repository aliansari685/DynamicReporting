namespace DynamicReporting.Mvc.ViewModels;

public sealed class SelectedColumnVm
{
    public string Table { get; set; } = string.Empty;

    public string Column { get; set; } = string.Empty;

    public string? DisplayName { get; set; }
}