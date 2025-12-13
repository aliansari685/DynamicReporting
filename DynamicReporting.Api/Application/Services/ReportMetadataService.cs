namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<string> GetAllTableNames() => unitOfWork.GetAllFullTableNames();

    public List<TableMetadata> GetAllMetadata() => unitOfWork.GetAllMetadata();

    public TableMetadata GetTableMetadata(string tableName)
    {
        var entityType = unitOfWork.GetTrustEntityType(tableName);

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties().Select(p => p.GetColumnName()).ToList()
        };
    }
}