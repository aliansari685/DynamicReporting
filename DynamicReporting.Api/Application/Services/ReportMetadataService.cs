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

    public List<TableMetadata> GetAllMetadata() => uow.GetAllMetadata();

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

        // دریافت لیست جدول‌های موجود در SelectedColumns
        var selectedTables = reportDefineEntity.SelectedColumns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var result = new List<SortableColumn>();
        var processedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // دریافت تمام EntityType ها یکبار برای بهبود Performance
        var allEntityTypes = uow.DbContext.Model.GetEntityTypes()
            .ToDictionary(e => e.GetTableName()!, StringComparer.OrdinalIgnoreCase);

        foreach (var tableName in selectedTables)
        {
            if (!allEntityTypes.TryGetValue(tableName, out var entityType))
                continue;

            var clrType = entityType.ClrType;

            // دریافت تمام ستون‌های Selected برای این جدول
            var selectedColumnsForTable = reportDefineEntity.SelectedColumns
                .Where(c => c.Table.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Column)
                .ToList();

            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName();

                if (!selectedColumnsForTable.Any(c => c.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidDataException("اطلاعات ارسال با پایگاه داده مغایرت است");

                var key = $"{tableName}.{columnName}";

                // جلوگیری از تکراری شدن ستون‌ها
                if (!processedColumns.Add(key))
                    continue;

                var displayName = ExtensionMethods.GetDescriptionFromSwaggerSchemaAttribute(clrType, property.Name);

                // اگر نام فارسی برای ستون وجود نداشت، از نام انگلیسی استفاده کن
                if (string.IsNullOrEmpty(displayName))
                    displayName = columnName;

                result.Add(new SortableColumn
                {
                    Table = tableName,
                    Column = columnName,
                    Field = $"{tableName}.{columnName}",
                    DisplayName = displayName
                });
            }
        }

        //// مرتب‌سازی بر اساس TableName و سپس DisplayName
        //return result
        //    .OrderBy(c => c.Field.Split('.')[0])  // مرتب‌سازی بر اساس TableName
        //    .ThenBy(c => c.DisplayName)            // سپس بر اساس DisplayName
        //    .ToList();

        return result;
    }

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

            var selectedColumnsForTable = reportDefineEntity.SelectedColumns
                .Where(c => c.Table.Equals(table, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Column)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            List<DisplayColumnsMetadata> columns = entityType.GetProperties()
                .Where(p => selectedColumnsForTable.Contains(p.GetColumnName()))
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


    private async Task<ReportDefinition> GetReportDefinitionAsync(int reportDefinitionId)
    {
        var report = await uow.DbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }

}