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
            Columns = entityType.GetProperties().Select(p => p.GetColumnName()).ToList()
        };
    }

    /// <summary>
    /// دریافت لیست نام تمام جدول‌های موجود در مدل EF Core.
    /// </summary>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>لیستی از نام جدول‌ها.</returns>
    public static List<string> GetAllModelsNames(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.ClrType.Name).Distinct().ToList();


    public static List<string> GetAllTableNames(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.GetTableName()!).Distinct().ToList();


    public static List<string> GetAllTableNames1(this IUnitOfWork unitOfWork)
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
    public static List<TableMetadata> GetAllMetadata(this IUnitOfWork unitOfWork) =>
        unitOfWork.DbContext.Model.GetEntityTypes()
            .Select(e => new TableMetadata
            {
                TableName = e.GetTableName()!,
                Columns = e.GetProperties().Select(p => p.GetColumnName()).ToList()
            }).ToList();


}
