namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<DisplayMetadata> GetAllTableNames()
    {
        List<DisplayMetadata> displays = [];
        var tablesName = unitOfWork.GetAllFullTableNames();

        displays.AddRange(tablesName.Select(unitOfWork.GetTrustEntityType).Select(entityType => new DisplayMetadata
        {
            PhysicalName = entityType.GetTableName()!,
            DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType)
        }));
        return displays;
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