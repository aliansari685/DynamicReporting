namespace DynamicReporting.Mvc.Application.Interfaces;

/// <summary>
/// Service contract for communicating with DynamicReporting API.
/// </summary>
public interface IMetadataService
{
    /// <summary>
    /// Gets all table names from the database.
    /// </summary>
    Task<IReadOnlyList<DisplayTableVm>> GetAllTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed Metadata for all tables.
    /// </summary>
    Task<IReadOnlyList<TableMetadataVm>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets Metadata for a specific table.
    /// </summary>
    /// <param name="tableName">The physical table name.</param>
    Task<TableMetadataVm?> GetTableMetadataAsync(string tableName);
}