namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(IUnitOfWork unitOfWork) : IReportDataService
{
    private ShopTestDbContext Db => unitOfWork.DbContext;

    public async Task<List<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId)
    {
        var report = await Db.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        if (report == null)
            throw new KeyNotFoundException($"ReportDefinition {reportDefinitionId} not found");

        if (report.SelectedColumns == null || !report.SelectedColumns.Any())
            throw new InvalidOperationException("No selected columns defined for report");

        var baseTable = report.BaseTable;

        // SELECT
        var selectClause = string.Join(", ",
            report.SelectedColumns.Select(c =>
                $"[{c.Table}].[{c.Column}] AS [{c.Table}_{c.Column}]"));

        // JOIN
        var joinClause = BuildDynamicJoins(baseTable, report.SelectedColumns.ToList());

        var sql = $"""
                   SELECT
                   TOP (100000)
                   {selectClause}
                   FROM [{baseTable}]
                   {joinClause}
                   """;

        return await ExecuteDynamicSqlAsync(sql);
    }

    private string BuildDynamicJoins(string baseTable, List<SelectedColumn> columns)
    {
        var model = Db.Model;

        var baseEntity = model.GetEntityTypes()
            .FirstOrDefault(e =>
                e.GetTableName()!.Equals(baseTable, StringComparison.OrdinalIgnoreCase));

        if (baseEntity == null)
            throw new InvalidOperationException($"Base table '{baseTable}' not found in EF model");

        var joins = new List<string>();

        var joinedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            baseTable
        };

        var targetTables = columns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(t => !t.Equals(baseTable, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var table in targetTables)
        {
            if (joinedTables.Contains(table))
                continue;

            var targetEntity = model.GetEntityTypes()
                .FirstOrDefault(e =>
                    e.GetTableName()!.Equals(table, StringComparison.OrdinalIgnoreCase));

            if (targetEntity == null)
                throw new InvalidOperationException($"Table '{table}' not found in EF model");

            IEntityType? startEntity = null;

            foreach (var jt in joinedTables)
            {
                startEntity = model.GetEntityTypes()
                    .FirstOrDefault(e =>
                        e.GetTableName()!.Equals(jt, StringComparison.OrdinalIgnoreCase));

                if (startEntity != null)
                    break;
            }

            if (startEntity == null)
                throw new InvalidOperationException("No joined table entity found");

            var path = FindJoinPath(startEntity, targetEntity);

            foreach (var fk in path)
            {
                var principalTable = fk.PrincipalEntityType.GetTableName()!;
                var dependentTable = fk.DeclaringEntityType.GetTableName()!;

                var principalKey = fk.PrincipalKey.Properties.First().GetColumnName();
                var foreignKey = fk.Properties.First().GetColumnName();

                // 🔴 نکته حیاتی: JOIN باید از جدول جوین ‌شده به جدول جدید باشد
                string leftTable, rightTable, leftColumn, rightColumn;

                if (joinedTables.Contains(dependentTable))
                {
                    leftTable = dependentTable;
                    leftColumn = foreignKey;
                    rightTable = principalTable;
                    rightColumn = principalKey;
                }
                else if (joinedTables.Contains(principalTable))
                {
                    leftTable = principalTable;
                    leftColumn = principalKey;
                    rightTable = dependentTable;
                    rightColumn = foreignKey;
                }
                else
                {
                    // مسیر اشتباه → این یعنی ترتیب Join خراب شده
                    throw new InvalidOperationException(
                        $"Join order error between {principalTable} and {dependentTable}");
                }

                if (!joinedTables.Contains(rightTable))
                {
                    joins.Add(
                        $"LEFT JOIN [{rightTable}] ON [{rightTable}].[{rightColumn}] = [{leftTable}].[{leftColumn}]");

                    joinedTables.Add(rightTable);
                }
            }
        }

        return string.Join(Environment.NewLine, joins);
    }

    private List<IReadOnlyForeignKey> FindJoinPath(
        IEntityType from,
        IEntityType to)
    {
        var visited = new HashSet<IEntityType>();
        var queue = new Queue<(IEntityType Entity, List<IReadOnlyForeignKey> Path)>();

        queue.Enqueue((from, []));
        visited.Add(from);

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            if (current == to)
                return path;

            // FK خروجی (Dependent → Principal)
            foreach (var fk in current.GetForeignKeys())
            {
                var next = fk.PrincipalEntityType;
                if (visited.Add(next))
                {
                    var newPath = new List<IReadOnlyForeignKey>(path) { fk };
                    queue.Enqueue((next, newPath));
                }
            }

            // FK ورودی (Principal ← Dependent)
            foreach (var fk in current.GetReferencingForeignKeys())
            {
                var next = fk.DeclaringEntityType;
                if (visited.Add(next))
                {
                    var newPath = new List<IReadOnlyForeignKey>(path) { fk };
                    queue.Enqueue((next, newPath));
                }
            }
        }

        // ❌ بدون FK = بدون گزارش
        throw new InvalidOperationException(
            $"No FK join path exists between '{from.GetTableName()}' and '{to.GetTableName()}'.");
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
        var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(reader.FieldCount);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var raw = reader.IsDBNull(i) ? null : reader.GetValue(i);
                row[columnNames[i]] = raw;
            }

            result.Add(row);
        }

        return result;
    }
}