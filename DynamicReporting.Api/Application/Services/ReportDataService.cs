namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(IUnitOfWork unitOfWork) : IReportDataService
{
    private ShopTestDbContext Db => unitOfWork.DbContext;

    public async Task<List<Dictionary<string, object>>> GetReportDataAsync(int reportDefinitionId)
    {
        // دریافت گزارش
        var report = await Db.Set<ReportDefinition>()
            .Where(r => r.Id == reportDefinitionId)
            .AsNoTracking()
            .FirstOrDefaultAsync();


        if (report == null)
            throw new KeyNotFoundException($"ReportDefinition {reportDefinitionId} not found");

        if (report.SelectedColumns == null || !report.SelectedColumns.Any())
            throw new Exception("No selected columns found.");

        var baseTable = report.BaseTable;

        // SELECT
        var selectClause = string.Join(", ",
            report.SelectedColumns.Select(x =>
                $"{x.Table}.{x.Column} AS [{x.Table}_{x.Column}]"));

        // JOINs
        var joinClause = BuildDynamicJoins(baseTable, report.SelectedColumns.ToList());

        //todo: مشکل اینه ک تو دیتابیس اسم مدل ذخیره شده ن اسم جدول حالا ببین توی ثبتش کدومو میزنیم اونی ک اس داره یا اون ک بدون اس

        var sql = $@"
            SELECT {selectClause}
            FROM {baseTable}
            {joinClause}
        ";

        return (await ExecuteDynamicSqlAsync(sql))!;
    }
    private string BuildDynamicJoins(string baseTable, List<SelectedColumn> columns)
    {
        var model = Db.Model;

        // موجودیت جدول پایه
        var baseEntity = model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName()!.Equals(baseTable, StringComparison.OrdinalIgnoreCase)); // db name example : orders

        //var baseEntity = model.GetEntityTypes()
        //    .FirstOrDefault(e =>
        //        e.ClrType.Name.Equals(baseTable, StringComparison.OrdinalIgnoreCase)// dbset name for example : order
        //    );

        if (baseEntity == null)
            throw new NullReferenceException($"Base table '{baseTable}' not found in EF model.");

        var joins = new List<string>();

        // همه جدول‌های دیگر
        var otherTables = columns
            .Select(c => c.Table)
            .Where(t => !t.Equals(baseTable, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        foreach (var tbl in otherTables)
        {
            // موجودیت مربوط به جدول مورد نیاز

            var otherEntity = model.GetEntityTypes()
                .FirstOrDefault(e =>
                        e.GetTableName()!.Equals(tbl, StringComparison.OrdinalIgnoreCase)
                );

            if (otherEntity == null)
                throw new Exception($"Table '{tbl}' not found in EF model.");

            // پیدا کردن FK بین baseTable و tbl
            var fk = baseEntity.GetForeignKeys()
                .FirstOrDefault(f => f.PrincipalEntityType == otherEntity);

            if (fk == null)
            {
                fk = otherEntity.GetForeignKeys()
                    .FirstOrDefault(f => f.PrincipalEntityType == baseEntity);

                if (fk == null)
                    throw new Exception($"No FK relation found between {baseTable} and {tbl}");

                // اگر FK از طرف جدول دیگر بود (Relation معکوس)
                var principalKey = fk.PrincipalKey.Properties.First().GetColumnName();
                var foreignKey = fk.Properties.First().GetColumnName();

                joins.Add(
                    $"LEFT JOIN {tbl} ON {tbl}.{principalKey} = {baseTable}.{foreignKey}"
                );
            }
            else
            {
                // FK از طرف جدول پایه بود
                var principalKey = fk.PrincipalKey.Properties.First().GetColumnName();
                var foreignKey = fk.Properties.First().GetColumnName();

                joins.Add(
                    $"LEFT JOIN {tbl} ON {tbl}.{principalKey} = {baseTable}.{foreignKey}"
                );
            }
        }

        return string.Join(Environment.NewLine, joins);
    }

    private async Task<List<Dictionary<string, object?>>> ExecuteDynamicSqlAsync(string sql)
    {
        var result = new List<Dictionary<string, object?>>();

        await using var conn = Db.Database.GetDbConnection();
        await using var command = conn.CreateCommand();

        command.CommandText = sql;

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

        var columnNames = Enumerable.Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .ToArray();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(reader.FieldCount);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var raw = reader.IsDBNull(i) ? null : reader.GetValue(i);
                row[columnNames[i]] = raw;
            }

            result.Add(row);
        }
        return result;
    }

}