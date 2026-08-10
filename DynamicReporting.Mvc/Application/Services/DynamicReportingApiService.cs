namespace DynamicReporting.Mvc.Application.Services;

/// <summary>
/// Service for communicating with ReportMetadata API
/// </summary>
public class DynamicReportingApiService(HttpClient httpClient) : IDynamicReportingApiService
{
    /// <summary>
    /// Get all table names from the database
    /// </summary>
    public async Task<List<DisplayTableDto>> GetAllTablesAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/report-metadata/tables");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DisplayTableDto>>(json) ?? [];
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching all tables");
            throw;
        }
    }

    /// <summary>
    /// Get detailed metadata for all tables
    /// </summary>
    public async Task<List<TableMetadataDto>> GetAllMetadataAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/report-metadata/metadata");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TableMetadataDto>>(json) ?? [];
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching all metadata");
            throw;
        }
    }

    /// <summary>
    /// Get metadata for a specific table
    /// </summary>
    public async Task<TableMetadataDto?> GetTableMetadataAsync(string tableName)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/report-metadata/metadata/{tableName}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TableMetadataDto>(json);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Log.Warning("Table '{TableName}' not found", tableName);
            return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error fetching metadata for table '{tableName}'");
            throw;
        }
    }
}