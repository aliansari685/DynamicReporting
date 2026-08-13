namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportGeneratedService(
    HttpClient httpClient) : IReportGeneratedService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ReportGenerationVm> GetGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/report-generated/{id}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   ReportGenerationVm>(
                   JsonOptions,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "اطلاعات گزارش تولید شده دریافت نشد.");
    }

    public async Task<IReadOnlyList<ReportGenerationVm>>
        GetGeneratedReportsAsync(
            CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            "api/report-generated",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<ReportGenerationVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    public async Task<string> GetGeneratedReportStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/report-generated/status/{id}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(
            cancellationToken);
    }

    public async Task<HttpResponseMessage> DownloadGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetAsync(
            $"api/report-generated/download/{id}",
            cancellationToken);
    }

    public async Task<bool> DeleteGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(
            $"api/report-generated/{id}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }
}