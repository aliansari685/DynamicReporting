namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<string> GetAllTableNames()
    {
        //todo : add persian title name 
        return unitOfWork.GetAllFullTableNames();
    }

    public List<TableMetadata> GetAllMetadata() => unitOfWork.GetAllMetadata();

    public TableMetadata GetTableMetadata(string tableName)
    {
        var entityType = unitOfWork.GetTrustEntityType(tableName);

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties()
                .Select(p => new DisplayMetadata
                {
                    PhysicalName = p.GetColumnName(),
                    DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType, p.Name)
                }).ToList()
        };
    }
}