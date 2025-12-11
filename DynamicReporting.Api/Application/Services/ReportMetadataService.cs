namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<string> GetAllTableNames()
    {
        var res = unitOfWork.GetAllModelsNames();
        var res1 = unitOfWork.GetAllModelsNames1();
        var res2 = unitOfWork.GetAllTableNames();

        return unitOfWork.GetAllTableNames();
    }

    public List<TableMetadata> GetAllMetadata() => unitOfWork.GetAllMetadata();

    public TableMetadata GetTableMetadata(string tableName)
    {
        var entityType = unitOfWork.DbContext.Model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName()?.Equals(tableName, StringComparison.OrdinalIgnoreCase) ?? false);

        if (entityType == null)
            throw new KeyNotFoundException($"جدول {tableName} نا معتبر است");

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties().Select(p => p.GetColumnName()).ToList()
        };
    }
}