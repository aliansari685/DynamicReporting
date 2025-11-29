using DynamicReporting.Api.Shared.Helper;

namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<string> GetAllTableNames()
    {
        return unitOfWork.GetAllTableNames();
    }

    public List<TableMetadata> GetAllMetadata()
    {
        return unitOfWork.GetAllMetadata();
    }

    public TableMetadata GetTableMetadata(string tableName)
    {
        var entityType = unitOfWork.DbContext.Model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName()?.Equals(tableName, StringComparison.OrdinalIgnoreCase) ?? false);

        if (entityType == null)
            throw new KeyNotFoundException($"Table {tableName} not found in DbContext.");

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties().Select(p => p.GetColumnName()).ToList()
        };
    }
}