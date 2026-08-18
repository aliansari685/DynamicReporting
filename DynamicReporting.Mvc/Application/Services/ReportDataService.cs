namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportDataService(
    HttpClient httpClient) : IReportDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedReportDataVm> GetReportDataAsync(
        int reportDefinitionId,
        string? filters = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"page={page}",
            $"take={take}"
        };

        if (!string.IsNullOrWhiteSpace(filters))
            query.Add(
                $"filters={Uri.EscapeDataString(filters)}");

        if (!string.IsNullOrWhiteSpace(sort))
            query.Add(
                $"sort={Uri.EscapeDataString(sort)}");

        if (!string.IsNullOrWhiteSpace(dir))
            query.Add(
                $"dir={Uri.EscapeDataString(dir)}");

        var url =
            $"api/report-data/{reportDefinitionId}?{string.Join("&", query)}";

        var response = await httpClient.GetAsync(
            url,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PagedReportDataVm>(
                   JsonOptions,
                   cancellationToken)
               ?? new PagedReportDataVm();
    }

    public async Task<IReadOnlyList<FilterableColumnVm>>
        GetFilterableColumnsAsync(
            int reportDefinitionId,
            CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/report-data/{reportDefinitionId}/filterable-columns",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<FilterableColumnVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    public async Task<IReadOnlyList<SortableColumnVm>>
        GetSortableColumnsAsync(
            int reportDefinitionId,
            CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/report-data/{reportDefinitionId}/sortable-columns",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<SortableColumnVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }
}
