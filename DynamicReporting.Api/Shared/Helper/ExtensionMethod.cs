namespace DynamicReporting.Api.Shared.Helper;

public static class ExtensionMethods
{
    /// <summary>
    ///     دریافت نام تمام پراپرتی‌های عمومی یک مدل جنریک.
    /// </summary>
    /// <typeparam name="T">نوع مدل مورد نظر.</typeparam>
    /// <returns>لیستی از نام پراپرتی‌ها.</returns>
    public static List<string> GetPropertyNames<T>()
    {
        return GetPropertyNames(typeof(T));
    }

    /// <summary>
    ///     دریافت نام تمام پراپرتی‌های عمومی یک نوع داده مشخص.
    /// </summary>
    /// <param name="modelType">نوع مدل برای استخراج پراپرتی‌ها.</param>
    /// <returns>لیستی از نام پراپرتی‌ها.</returns>
    public static List<string> GetPropertyNames(Type modelType)
    {
        return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();
    }

    /// <summary>
    ///     دریافت اطلاعات جدول (Table Metadata) برای یک موجودیت مشخص.
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
                .Select(p => new DisplayMetadata
                {
                    PhysicalName = p.GetColumnName(),
                    DisplayName = GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType, p.Name)
                }).ToList()
        };
    }

    /// <summary>
    ///     دریافت لیست نام تمام جدول‌های موجود در مدل مثل customer
    /// </summary>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>لیستی از نام جدول‌ها.</returns>
    public static List<string> GetAllModelsNames(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.ClrType.Name).Distinct().ToList();
    }


    /// <summary>
    ///     خروجی دقیقه نام جدول های اصلی مثل customers
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns>خروجی  نام جدول های اصلی</returns>
    public static List<string> GetAllShortTableNames(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model.GetEntityTypes().Select(e => e.GetTableName()!).Distinct().ToList();
    }


    /// <summary>
    ///     اسم جدول رو میدیم و موجودیت درست در صورت وجود داشتن دریافت میکنیم
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
    ///     خروجی دقیقه نام جدول مثل customers
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns>خروجی دقیق نام تمامی جداول </returns>
    public static List<string> GetAllFullTableNames(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetTableMappings())
            .Where(e => e.Table.Name != nameof(ShopTestDbContext.ReportDefinitions) &&
                        e.Table.Name != nameof(ShopTestDbContext.ReportGenerations))
            .Select(m => m.Table.Name)
            .Distinct()
            .ToList();
    }

    /// <summary>
    ///     دریافت اطلاعات کامل تمام جدول‌ها شامل نام جدول و ستون‌ها.
    /// </summary>
    /// <param name="unitOfWork">اینترفیس UnitOfWork شامل DbContext.</param>
    /// <returns>لیستی از TableMetadata برای همه موجودیت‌ها.</returns>
    public static List<TableMetadata> GetAllMetadata(this IUnitOfWork unitOfWork)
    {
        return unitOfWork.DbContext.Model.GetEntityTypes()
            .Where(e => e.GetTableName() != nameof(ShopTestDbContext.ReportDefinitions))
            .Select(entity =>
            {
                //   var entityName = entity.ClrType.Name;
                var clrType = entity.ClrType;

                return new TableMetadata
                {
                    TableName = entity.GetTableName()!,
                    Columns = entity.GetProperties()
                        .Select(p => new DisplayMetadata
                        {
                            PhysicalName = p.GetColumnName(),
                            DisplayName = GetDescriptionFromSwaggerSchemaAttribute(clrType, p.Name)
                        })
                        .ToList()
                };
            })
            .ToList();
    }

    /// <summary>
    ///     دریافت توضیحات (Description) از ویژگی SwaggerSchemaAttribute برای یک خاصیت مشخص از نوع داده شده
    /// </summary>
    /// <param name="clrType">نوع کلاس حاوی خاصیت</param>
    /// <param name="propertyName">نام خاصیت مورد نظر</param>
    /// <returns>مقدار توضیحات در صورت وجود، در غیر این صورت null</returns>
    /// <remarks>
    ///     این متد از طریق Reflection به خاصیت مورد نظر دسترسی پیدا کرده و در صورت وجود ویژگی SwaggerSchemaAttribute،
    ///     مقدار Description آن را باز می‌گرداند
    /// </remarks>
    public static string? GetDescriptionFromSwaggerSchemaAttribute(Type clrType, string propertyName)
    {
        var propertyInfo = clrType.GetProperty(propertyName);
        var swaggerAttr = propertyInfo?
            .GetCustomAttribute<SwaggerSchemaAttribute>();
        return swaggerAttr?.Description;
    }

    /// <summary>
    ///     دریافت توضیحات (Description) از ویژگی SwaggerSchemaAttribute برای یک نوع (کلاس) مشخص
    /// </summary>
    /// <param name="clrType">نوع کلاسی که ویژگی روی آن قرار گرفته</param>
    /// <returns>مقدار توضیحات در صورت وجود، در غیر این صورت null</returns>
    /// <remarks>
    ///     این متد مستقیماً ویژگی SwaggerSchemaAttribute را از روی نوع (Type) دریافت کرده
    ///     و مقدار Description آن را برمی‌گرداند
    /// </remarks>
    public static string? GetDescriptionFromSwaggerSchemaAttribute(Type clrType)
    {
        var swaggerAttr = clrType.GetCustomAttribute<SwaggerSchemaAttribute>();
        return swaggerAttr?.Description;
    }

    /// <summary>
    ///     تبدیل وضعیت (State) Hangfire به رشته فارسی معادل آن
    /// </summary>
    /// <param name="state">وضعیت Hangfire به صورت رشته انگلیسی</param>
    /// <returns>معادل فارسی وضعیت</returns>
    /// <remarks>
    ///     این متد یک متد الحاقی (Extension Method) برای رشته‌ها است و وضعیت‌های مختلف Hangfire را
    ///     به معادل فارسی ترجمه می‌کند. در صورتی که وضعیت نامشخص باشد، مقدار "نامشخص" بازگردانده می‌شود.
    /// </remarks>
    /// <example>
    ///     <code>
    /// var state = "Enqueued";
    /// var persianState = state.HangfireStateToPersian(); // خروجی: "در صف انتظار"
    /// </code>
    /// </example>
    public static string HangfireStateToPersian(this string state)
    {
        var convert = Enum.Parse(typeof(HangfireJobQueueService.HangfireJobState), state);
        return convert switch
        {
            HangfireJobQueueService.HangfireJobState.Enqueued => "در صف انتظار",
            HangfireJobQueueService.HangfireJobState.Processing => "در حال پردازش",
            HangfireJobQueueService.HangfireJobState.Succeeded => "با موفقیت انجام شد",
            HangfireJobQueueService.HangfireJobState.Failed => "ناموفق",
            HangfireJobQueueService.HangfireJobState.Scheduled => "زمان‌بندی شده",
            HangfireJobQueueService.HangfireJobState.Deleted => "حذف شده",
            HangfireJobQueueService.HangfireJobState.Awaiting => "در انتظار",
            HangfireJobQueueService.HangfireJobState.AwaitingContinuation => "در انتظار ادامه پردازش",
            _ => "نامشخص"
        };
    }
}