namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportGeneratedService(
    HttpClient httpClient,
    IReportDefinitionService definitionService) : IReportGeneratedService
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

        var reportGenerationVm = await response.Content.ReadFromJsonAsync<
                                     ReportGenerationVm>(
                                     JsonOptions,
                                     cancellationToken)
                                 ?? throw new InvalidOperationException(
                                     "اطلاعات گزارش تولید شده دریافت نشد.");
        reportGenerationVm.ReportDefinitionName =
            await GetReportDefinitionNameAsync(reportGenerationVm.ReportDefinitionId, cancellationToken);
        return reportGenerationVm;
    }

    public async Task<IReadOnlyList<ReportGenerationVm>>
        GetGeneratedReportsAsync(
            CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            "api/report-generated",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var reportGenerationVms = await response.Content.ReadFromJsonAsync<
                                      List<ReportGenerationVm>>(
                                      JsonOptions,
                                      cancellationToken)
                                  ?? [];
        foreach (var reportGenerationVm in reportGenerationVms)
            reportGenerationVm.ReportDefinitionName =
                await GetReportDefinitionNameAsync(reportGenerationVm.ReportDefinitionId, cancellationToken);
        return reportGenerationVms;
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

    public async Task<HttpResponseMessage> DownloadGeneratedReportAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetAsync($"api/report-generated/download/{id}",
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

    private async Task<string> GetReportDefinitionNameAsync(int reportDefinitionId, CancellationToken cancellationToken)
    {
        var report = await definitionService.GetReportAsync(reportDefinitionId, cancellationToken);
        return report?.Name ?? "";
    }
}