namespace DynamicReporting.Mvc.ViewModels;

public sealed class SortableColumnVm
{
    public string Table { get; init; } = string.Empty;

    public string Column { get; init; } = string.Empty;

    public string Field { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;
}