namespace DynamicReporting.Api.Domain.Interfaces;

public class ReportDataService(IUnitOfWork unitOfWork) : IReportDataService
{
    public async Task<List<Dictionary<string, object>>> GetReportDataAsync(int reportDefinitionId)
    {
        // 1. دریافت ReportDefinition
        var repo = unitOfWork.Repository<ReportDefinition>();
        var report = await repo.GetByIdAsync(reportDefinitionId);

        if (report == null)
            throw new KeyNotFoundException($"ReportDefinition {reportDefinitionId} not found");

        var baseTable = report.BaseTable;
        var selected = report.SelectedColumns;

        if (selected == null || !selected.Any())
            throw new Exception("No selected columns found.");

        // 2. ساخت SELECT داینامیک
        var selectParts = selected
            .Select(x => $"{x.Table}.{x.Column} AS [{x.Table}_{x.Column}]")
            .ToList();

        var selectClause = string.Join(", ", selectParts);

        // 3. ساخت JOINهای داینامیک (ساده: بر اساس CustomerId)
        // این‌جا بعداً می‌توانیم هوشمندش کنیم
        var joinClause = BuildDynamicJoins(baseTable, selected);

        // 4. ساخت SQL نهایی
        var sql = $@"
            SELECT {selectClause}
            FROM {baseTable}
            {joinClause}
        ";

        // 5. اجرای Raw SQL
        var rows = new List<Dictionary<string, object>>();

        await using var command = unitOfWork.DbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;

        await unitOfWork.DbContext.Database.OpenConnectionAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = (reader.IsDBNull(i) ? null : reader.GetValue(i)) ?? throw new InvalidOperationException();
            }

            rows.Add(row);
        }

        return rows;
    }

    private string BuildDynamicJoins(string baseTable, List<SelectedColumn> columns)
    {
        var joins = new List<string>();

        var otherTables = columns
            .Where(c => !c.Table.Equals(baseTable, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Table)
            .Distinct()
            .ToList();

        // مثال ساده Join
        foreach (var tbl in otherTables)
        {
            joins.Add($"LEFT JOIN {tbl} ON {tbl}.Id = {baseTable}.{tbl}Id");
        }

        return string.Join(Environment.NewLine, joins);
    }
}