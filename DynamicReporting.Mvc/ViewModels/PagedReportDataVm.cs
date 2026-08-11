namespace DynamicReporting.Mvc.ViewModels;

public sealed class PagedReportDataVm
{
    public List<Dictionary<string, object?>> Items { get; set; } = [];

    public int Page { get; set; }

    public int Take { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages =>
        Take <= 0
            ? 0
            : (int)Math.Ceiling((double)TotalCount / Take);
}