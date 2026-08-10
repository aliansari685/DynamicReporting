namespace DynamicReporting.Mvc.Application.Interfaces;

/// <summary>
/// Service contract for communicating with DynamicReporting API.
/// </summary>
public interface IDynamicReportingApiService
{
    /// <summary>
    /// Gets all table names from the database.
    /// </summary>
    Task<List<DisplayTableDto>> GetAllTablesAsync();

    /// <summary>
    /// Gets detailed metadata for all tables.
    /// </summary>
    Task<List<TableMetadataDto>> GetAllMetadataAsync();

    /// <summary>
    /// Gets metadata for a specific table.
    /// </summary>
    /// <param name="tableName">The physical table name.</param>
    Task<TableMetadataDto?> GetTableMetadataAsync(string tableName);
}