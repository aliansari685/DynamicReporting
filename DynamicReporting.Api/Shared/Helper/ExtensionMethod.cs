namespace DynamicReporting.Api.Shared.Helper;

public static class ExtensionMethods
{
    /// <summary>
    /// دریافت نام تمام پراپرتی‌های عمومی یک مدل جنریک.
    /// </summary>
    /// <typeparam name="T">نوع مدل مورد نظر.</typeparam>
    /// <returns>لیستی از نام پراپرتی‌ها.</returns>
    public static List<string> GetPropertyNames<T>() => GetPropertyNames(typeof(T));

    /// <summary>
    /// دریافت نام تمام پراپرتی‌های عمومی یک نوع داده مشخص.
    /// </summary>
    /// <param name="modelType">نوع مدل برای استخراج پراپرتی‌ها.</param>
    /// <returns>لیستی از نام پراپرتی‌ها.</returns>
    public static List<string> GetPropertyNames(Type modelType) =>
        modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();

    /// <summary>
    /// دریافت اطلاعات جدول (Table Metadata) برای یک موجودیت مشخص.
    /// </summary>
    /// <typeparam name="T">نوع موجودیت EF Core.</typeparam>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>شیء شامل نام جدول و ستون‌ها.</returns>
    /// <exception cref="NullReferenceException">درصورتی‌که موجودیت در مدل دیتابیس یافت نشود.</exception>
    public static TableMetadata GetTableInfo<T>(this IUnitOfWork unitOfWork) where T : class
    {
        var entityType = unitOfWork.DbContext.Model.FindEntityType(typeof(T))
                         ?? throw new NullReferenceException("موجودیت یافت نشد");

        return new TableMetadata
        {
            TableName = entityType.GetTableName()!,
            Columns = entityType.GetProperties()
                .Select(p => new ColumnMetadata
                {
                    ColumnName = p.GetColumnName(),
                    Title = GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType, p.Name)
                }).ToList()
        };
    }

    /// <summary>
    /// دریافت لیست نام تمام جدول‌های موجود در مدل مثل customer
    /// </summary>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>لیستی از نام جدول‌ها.</returns>
    public static List<string> GetAllModelsNames(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.ClrType.Name).Distinct().ToList();


    /// <summary>
    /// خروجی دقیقه نام جدول های اصلی مثل customers
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns>خروجی  نام جدول های اصلی</returns>
    public static List<string> GetAllShortTableNames(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.GetTableName()!).Distinct().ToList();


    /// <summary>
    /// اسم جدول رو میدیم و موجودیت درست در صورت وجود داشتن دریافت میکنیم
    /// </summary>
    /// <param name="unitOfWork">کانتکس</param>
    /// <param name="tableName">اسم جدول</param>
    /// <returns></returns>
    public static IEntityType GetTrustEntityType(this IUnitOfWork unitOfWork, string tableName)
    {
        var res = unitOfWork.DbContext.Model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName()?.Equals(tableName, StringComparison.OrdinalIgnoreCase) ?? false);

        return res ?? throw new KeyNotFoundException($"جدول {tableName} نا معتبر است");
    }

    /// <summary>
    /// خروجی دقیقه نام جدول مثل customers
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns>خروجی دقیق نام تمامی جداول </returns>
    public static List<string> GetAllFullTableNames(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetTableMappings())
            .Select(m => m.Table.Name)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// دریافت اطلاعات کامل تمام جدول‌ها شامل نام جدول و ستون‌ها.
    /// </summary>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>لیستی از TableMetadata برای همه موجودیت‌ها.</returns>
    public static List<TableMetadata> GetAllMetadata(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model.GetEntityTypes()
            .Select(entity =>
            {
                var clrType = entity.ClrType;

                return new TableMetadata
                {
                    TableName = entity.GetTableName()!,
                    Columns = entity.GetProperties()
                        .Select(p => new ColumnMetadata
                        {
                            ColumnName = p.GetColumnName(),
                            Title = GetDescriptionFromSwaggerSchemaAttribute(clrType, p.Name)
                        })
                        .ToList()
                };
            })
            .ToList();
    }

    public static string? GetDescriptionFromSwaggerSchemaAttribute(Type clrType, string propertyName)
    {
        var propertyInfo = clrType.GetProperty(propertyName);
        var swaggerAttr = propertyInfo?
            .GetCustomAttribute<SwaggerSchemaAttribute>();
        return swaggerAttr?.Description;
    }
}