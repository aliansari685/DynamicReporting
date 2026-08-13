namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportExportService(
    HttpClient httpClient) : IReportExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        return await httpClient.GetAsync(
            url,
            cancellationToken);
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

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        var response = await httpClient.GetAsync(
            url,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   ExportJobResultVm>(
                   JsonOptions,
                   cancellationToken)
               ?? new ExportJobResultVm();
    }
}