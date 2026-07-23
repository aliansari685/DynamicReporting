namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService(IFilterOperatorHelper filterOperatorHelper, IUnitOfWork uow) : IReportMetadataService
{
    public List<DisplayMetadata> GetAllTableNames()
    {
        List<DisplayMetadata> displays = [];
        var tablesName = uow.GetAllFullTableNames();

        displays.AddRange(tablesName.Select(uow.GetTrustEntityType).Select(entityType => new DisplayMetadata
        {
            PhysicalName = entityType.GetTableName()!,
            DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(entityType.ClrType)
        }));
        return displays;
    }

    public List<TableMetadata> GetAllMetadata()
    {
        return uow.GetAllMetadata();
    }

    public TableMetadata GetTableMetadata(string tableName)
    {
        var entityType = uow.GetTrustEntityType(tableName);

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


    public async Task<List<SortableColumn>> GetSortableColumnsAsync(int reportDefinitionId)
    {
        var reportDefineEntity = await GetReportDefinitionAsync(reportDefinitionId);

        var selectedTables = reportDefineEntity.SelectedColumns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var result = new List<SortableColumn>();
        var processedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var allEntityTypes = uow.DbContext.Model.GetEntityTypes()
            .ToDictionary(e => e.GetTableName()!, StringComparer.OrdinalIgnoreCase);

        foreach (var tableName in selectedTables)
        {
            if (!allEntityTypes.TryGetValue(tableName, out var entityType))
                throw new InvalidDataException("اطلاعات ارسالی با پایگاه داده مغایر است");

            var clrType = entityType.ClrType;

            foreach (var property in entityType.GetProperties())
            {
                //برای اینکه همه ی ستون ها رو نشون نده میتونیم هر ستون را با اتریبیوت NotSortable نشان گذاری کنیم
                //To not show all columns, we can mark each column with the NotSortable attribute.

                var columnName = property.GetColumnName();
                var key = $"{tableName}.{columnName}";

                //جلوگیری از ثبت موارد تکراری
                if (!processedColumns.Add(key))
                    continue;

                result.Add(new SortableColumn
                {
                    Table = tableName,
                    Column = columnName,
                    Field = key,
                    DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(clrType, property.Name)
                });
            }
        }

        return result;
    }
    //// مرتب‌سازی بر اساس TableName و سپس DisplayName
    //return result
    //    .OrderBy(c => c.Field.Split('.')[0])  // مرتب‌سازی بر اساس TableName
    //    .ThenBy(c => c.DisplayName)            // سپس بر اساس DisplayName
    //    .ToList();


    public async Task<List<TableDisplayMetadata>> GetFilterableColumnsAsync(int reportDefinitionId)
    {
        var reportDefineEntity = await GetReportDefinitionAsync(reportDefinitionId);

        var selectedTables = reportDefineEntity.SelectedColumns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var result = new List<TableDisplayMetadata>();

        foreach (var table in selectedTables)
        {
            var entityType = uow.DbContext.Model.GetEntityTypes()
                .First(e => e.GetTableName() == table);

            var clrType = entityType.ClrType;

            var columns = entityType.GetProperties()
                .Select(p => new DisplayColumnsMetadata
                {
                    PhysicalName = p.GetColumnName(),
                    DisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(clrType, p.Name),
                    SupportedOperators = filterOperatorHelper.GetSupportedOperators(p.ClrType.Name)
                })
                .ToList();

            result.Add(new TableDisplayMetadata
            {
                TableName = entityType.GetTableName()!,
                TableDisplayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(clrType),
                Columns = columns
            });
        }

        return result;
    }

    public List<Dictionary<string, object?>> GetDisplayNameColumn(List<Dictionary<string, object?>> data)
    {
        var result = new List<Dictionary<string, object?>>();

        foreach (var row in data)
        {
            // ایجاد یک دیکشنری جدید برای ردیف جاری
            var newRow = new Dictionary<string, object?>(row);

            foreach (var (key, val) in row)
            {
                // جدا کردن نام جدول و ستون از کلید (فرمت: Table_Column)
                var parts = key.Split('.');
                var tableName = parts[0];
                var columnName = parts[1];

                var entityType = uow.GetTrustEntityType(tableName);

                var displayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(
                    entityType.ClrType,
                    columnName);

                // اگر DisplayName خالی بود، از نام ستون استفاده کن
                if (string.IsNullOrEmpty(displayName))
                    displayName = columnName;

                // اضافه کردن آیتم جدید به دیکشنری با کلید فارسی
                newRow[displayName] = val;
            }

            result.Add(newRow);
        }

        return result;
    }


    private async Task<ReportDefinition> GetReportDefinitionAsync(int reportDefinitionId)
    {
        var report = await uow.DbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }
}