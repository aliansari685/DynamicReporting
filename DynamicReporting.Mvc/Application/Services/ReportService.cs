namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportService(
    HttpClient httpClient) : IReportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    //todo : fix and complete

    public async Task<HttpResponseMessage> FastExportAsync(
        int reportDefinitionId,
        string? filters,
        string? sort,
        string? dir,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

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
            $"api/report-export/excel/fastExport/{reportDefinitionId}";

        if (query.Count > 0) url += "?" + string.Join("&", query);

        return await httpClient.GetAsync(
            url,
            cancellationToken);
    }


    public async Task<IReadOnlyList<ReportDefinitionVm>> GetReportsAsync(
        CancellationToken cancellationToken = default)
    {
        var response =
            await httpClient.GetAsync(
                "api/report-definitions",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<ReportDefinitionVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }


    public async Task<ReportDefinitionVm?> GetDefaultReportAsync(
        CancellationToken cancellationToken = default)
    {
        var response =
            await httpClient.GetAsync(
                "api/report-definitions/default",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
            ReportDefinitionVm>(
            JsonOptions,
            cancellationToken);
    }


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

        var response =
            await httpClient.GetAsync(
                url,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   PagedReportDataVm>(
                   JsonOptions,
                   cancellationToken)
               ?? new PagedReportDataVm();
    }


    public async Task<IReadOnlyList<ReportColumnVm>>
        GetFilterableColumnsAsync(
            int reportDefinitionId,
            CancellationToken cancellationToken = default)
    {
        var response =
            await httpClient.GetAsync(
                $"api/report-data/{reportDefinitionId}/filterable-columns",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<ReportColumnVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }


    public async Task<IReadOnlyList<ReportColumnVm>>
        GetSortableColumnsAsync(
            int reportDefinitionId,
            CancellationToken cancellationToken = default)
    {
        var response =
            await httpClient.GetAsync(
                $"api/report-data/{reportDefinitionId}/sortable-columns",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<ReportColumnVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    public async Task<ExportJobResultVm> ExportAsync(
        int reportDefinitionId,
        string? filters,
        string? sort,
        string? dir,
        string type,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(filters))
            query.Add(
                $"filters={Uri.EscapeDataString(filters)}");

        if (!string.IsNullOrWhiteSpace(sort))
            query.Add(
                $"sort={Uri.EscapeDataString(sort)}");

        if (!string.IsNullOrWhiteSpace(dir))
            query.Add(
                $"dir={Uri.EscapeDataString(dir)}");

        query.Add(
            $"type={Uri.EscapeDataString(type)}");

        var url =
            $"api/report-export/export/{reportDefinitionId}";

        if (query.Count > 0) url += "?" + string.Join("&", query);

        var response =
            await httpClient.GetAsync(
                url,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content
                   .ReadFromJsonAsync<ExportJobResultVm>(
                       cancellationToken)
               ?? new ExportJobResultVm();
    }
}