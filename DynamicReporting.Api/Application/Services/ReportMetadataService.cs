namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IUnitOfWork unitOfWork) : IReportMetadataService
{
    public List<DisplayMetadata> GetAllTableNames()
    {

        List<DisplayMetadata> displays = [];

        List<string> tablesName = unitOfWork.GetAllFullTableNames();

        foreach (string tableName in tablesName)
        {
            var entityType = unitOfWork.GetTrustEntityType(tableName);
            var r = new DisplayMetadata
            {
                PhysicalName = entityType.GetTableName()!,
                DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType)
            };
            displays.Add(r);
        }
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