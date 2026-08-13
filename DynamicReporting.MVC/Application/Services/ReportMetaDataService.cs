namespace DynamicReporting.Mvc.Application.Services;

public sealed class ReportMetadataService(
    HttpClient httpClient) : IMetadataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// دریافت تمام جداول دیتابیس
    /// </summary>
    public async Task<IReadOnlyList<DisplayTableVm>> GetAllTablesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            "api/report-metadata/tables",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<DisplayTableVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    /// <summary>
    /// دریافت متادیتای تمام جداول
    /// </summary>
    public async Task<IReadOnlyList<TableMetadataVm>> GetAllMetadataAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            "api/report-metadata/Metadata",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
                   List<TableMetadataVm>>(
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    /// <summary>
    /// دریافت متادیتای یک جدول مشخص
    /// </summary>
    public async Task<TableMetadataVm?> GetTableMetadataAsync(
        string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return null;

        var response = await httpClient.GetAsync(
            $"api/report-metadata/metadata/{Uri.EscapeDataString(tableName)}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<
            TableMetadataVm>(
            JsonOptions);
    }
}