namespace DynamicReporting.Mvc.ViewModels;

public sealed class FilterableColumnVm
{
    public string TableName { get; init; } = string.Empty;

    public string? TableDisplayName { get; init; }

    public IReadOnlyList<FilterColumnVm> Columns { get; init; } = [];
}

public sealed class FilterColumnVm
{
    public string PhysicalName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public IReadOnlyList<FilterOperatorVm> SupportedOperators { get; init; } = [];
}
