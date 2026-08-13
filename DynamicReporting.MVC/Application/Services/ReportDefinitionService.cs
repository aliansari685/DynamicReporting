namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportDefinitionService(
    HttpClient httpClient) : IReportDefinitionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ReportDefinitionVm?> GetReportAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"api/report-definitions/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
            ReportDefinitionVm>(
            JsonOptions,
            cancellationToken);
    }

    public async Task CreateReportAsync(
        ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "api/report-definitions",
            model,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateReportAsync(
        int id,
        ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/report-definitions/{id}",
            model,
            JsonOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteReportAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(
            $"api/report-definitions/{id}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }
    public async Task<IReadOnlyList<ReportDefinitionVm>> GetReportsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
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
        var response = await httpClient.GetAsync(
            "api/report-definitions/default",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
            ReportDefinitionVm>(
            JsonOptions,
            cancellationToken);
    }
}