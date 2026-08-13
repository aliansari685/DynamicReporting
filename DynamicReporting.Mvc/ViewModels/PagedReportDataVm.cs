namespace DynamicReporting.Mvc.ViewModels;

public sealed class PagedReportDataVm
{
    public IReadOnlyList<Dictionary<string, object?>> Data { get; init; } = [];

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int Take { get; init; }

    public int TotalPages { get; init; }

    public string? SortBy { get; init; }

    public string? Dir { get; init; }
}