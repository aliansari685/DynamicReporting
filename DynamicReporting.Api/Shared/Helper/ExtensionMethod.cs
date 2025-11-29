namespace DynamicReporting.Api.Shared.Helper;

public static class ExtensionMethods
{
    // Property Names
    public static List<string> GetPropertyNames<T>() => GetPropertyNames(typeof(T));

    public static List<string> GetPropertyNames(Type modelType) =>
        modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();

    // Table info از UnitOfWork
    public static TableMetadata GetTableInfo<T>(this IUnitOfWork unitOfWork) where T : class
    {
        var entityType = unitOfWork.DbContext.Model.FindEntityType(typeof(T))
                         ?? throw new NullReferenceException("موجودیت یافت نشد");

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties().Select(p => p.GetColumnName()).ToList()
        };
    }

    // تمام جدول‌ها
    public static List<string> GetAllTableNames(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.ClrType.Name).ToList();

    // تمام metadata
    public static List<TableMetadata> GetAllMetadata(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes()
            .Select(e => new TableMetadata
            {
                TableName = e.GetTableName()!,
                Columns = e.GetProperties().Select(p => p.GetColumnName()).ToList()
            }).ToList();
}