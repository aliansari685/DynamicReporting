namespace DynamicReporting.Mvc.ViewModels;

public sealed class FilterableColumnVm
{
    public string PhysicalName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public IReadOnlyList<FilterOperatorVm> SupportedOperators { get; init; } = [];
}