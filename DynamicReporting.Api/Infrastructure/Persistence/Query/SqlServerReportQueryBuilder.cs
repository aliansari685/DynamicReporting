namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

public sealed class SqlServerReportQueryBuilder(
    ISelectJoinBuilder builder,
    IQueryCacheManager cacheManager,
    IUnitOfWork uow) : IReportQueryBuilder
{
    public string BuildCountQuery(ReportDefinition report, string whereClause)
    {
        var template = GetQueryTemplate(report);

        return string.IsNullOrWhiteSpace(whereClause)
            ? $"""
               SELECT COUNT(1)
               FROM {template.FromClause}
               {template.JoinClause}
               """
            : $"""
               SELECT COUNT(1)
               FROM {template.FromClause}
               {template.JoinClause}
               {whereClause}
               """;
    }

    public (string whereClause, Dictionary<string, object> parameters) BuildWhereClause(List<FilterCondition>? filters)
    {
        if (filters == null || !filters.Any())
            return ("", new Dictionary<string, object>());

        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();
        var index = 0;

        foreach (var f in filters)
        {
            var paramName = $"@p{index}";

            var condition = f.Operator switch
            {
                "eq" => $"{f.Field} = {paramName}",
                "gt" => $"{f.Field} > {paramName}",
                "gte" => $"{f.Field} >= {paramName}",
                "lt" => $"{f.Field} < {paramName}",
                "lte" => $"{f.Field} <= {paramName}",
                "contains" => $"{f.Field} LIKE {paramName}",
                _ => throw new InvalidOperationException($"اپراتور {f.Operator} پشتیبانی نمی‌شود")
            };

            var value = f.Operator == "contains"
                ? $"%{f.Value}%"
                : f.Value;

            conditions.Add(condition);
            parameters.Add(paramName, value);
            index++;
        }

        var whereClause = conditions.Any()
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : "";

        return (whereClause, parameters);
    }

    public string BuildPagedQuery(ReportDefinition report, string whereClause, int page, int take,
        SortableColumnDto sortColumn)
    {
        var offset = (page - 1) * take;
        return BuildQuery(report, whereClause, offset, take, sortColumn);
    }

    public string BuildQuery(ReportDefinition report, string whereClause, int offset, int take,
        SortableColumnDto sortColumn)
    {
        var template = GetQueryTemplate(report);

        ValidateSortColumn(report, sortColumn);

        var fullSortColumn = $"[{sortColumn.Column!.Replace(".", "].[")}]";

        return string.IsNullOrWhiteSpace(whereClause)
            ? $"""
               SELECT
                   {template.SelectClause}
               FROM {template.FromClause}
               {template.JoinClause}
               ORDER BY {fullSortColumn} {sortColumn.SortDirection}
               OFFSET {offset} ROWS
               FETCH NEXT {take} ROWS ONLY
               """
            : $"""
               SELECT
                   {template.SelectClause}
               FROM {template.FromClause}
               {template.JoinClause}
               {whereClause}
               ORDER BY {fullSortColumn} {sortColumn.SortDirection}
               OFFSET {offset} ROWS
               FETCH NEXT {take} ROWS ONLY
               """;
    }

    #region Helper Method

    /// <summary>
    ///     ولیدیشن ستون و جدول ارسالی جهت مرتب سازی
    /// </summary>
    /// <param name="report"></param>
    /// <param name="sortColumn"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateSortColumn(ReportDefinition report, SortableColumnDto sortColumn)
    {
        // اگر کاربر ستونی ارسال نکرد، بر اساس کلید اصلی مرتب کن
        if (string.IsNullOrWhiteSpace(sortColumn.Column))
        {
            sortColumn.Column = $"{report.BaseTable}.{GetPrimaryKeyColumn(report.BaseTable)}";
            return;
        }

        var parts = sortColumn.Column.Split('.', StringSplitOptions.RemoveEmptyEntries);

        var tableName = parts[0];
        var columnName = parts[1];

        var tableExists = report.SelectedColumns.Any(x =>
            string.Equals(x.Table, tableName, StringComparison.OrdinalIgnoreCase));

        if (!tableExists)
            throw new ArgumentException($"جدول '{tableName}' در گزارش وجود ندارد.");

        var entityType = uow.GetTrustEntityType(tableName);

        var propertyExists = entityType.GetProperties().Any(p =>
            string.Equals(p.GetColumnName(), columnName, StringComparison.OrdinalIgnoreCase));

        if (!propertyExists)
            throw new ArgumentException($"ستون '{columnName}' در جدول '{tableName}' وجود ندارد.");
    }

    /// <summary>
    ///     متد کمکی برای دریافت ستون کلید اصلی
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetPrimaryKeyColumn(string tableName)
    {
        var entityType = uow.GetTrustEntityType(tableName);
        var primaryKey = entityType.FindPrimaryKey();

        return primaryKey == null ? "(SELECT NULL)" :
            primaryKey.Properties.Count == 1 ? primaryKey.Properties.First().GetColumnName() :
            string.Join(", ", primaryKey.Properties.Select(p => $"[{p.GetColumnName()}]"));
    }

    /// <summary>
    ///     بازگردانی template query برای یک گزارش شامل FROM, JOIN و SELECT clauses.
    ///     - ابتدا بررسی می‌کند که template از cache موجود باشد.
    ///     - اگر موجود نباشد، factory فراخوانی شده و template محاسبه و در cache ذخیره می‌شود.
    ///     - هدف: جلوگیری از محاسبات تکراری Join و Select در pagination و گزارش‌های بزرگ.
    ///     - از JoinPathResolver و SelectJoinBuilder برای محاسبه join و select استفاده می‌شود.
    /// </summary>
    /// <param name="report">گزارش مورد نظر که شامل BaseTable و SelectedColumns است</param>
    /// <returns>
    ///     Tuple شامل:
    ///     - FromClause: جدول پایه با کروشه SQL `[TableName]`
    ///     - JoinClause: رشته JOIN بین جدول پایه و جداول مرتبط
    ///     - SelectClause: رشته SELECT شامل ستون‌های انتخاب‌شده با alias
    /// </returns>
    private (string FromClause, string JoinClause, string SelectClause) GetQueryTemplate(ReportDefinition report)
    {
        return cacheManager.GetOrCreate(report.Id, () =>
        {
            var baseTable = report.BaseTable;
            var joinClause = builder.BuildJoinClause(baseTable, report.SelectedColumns, uow.GetTrustEntityType);
            var selectClause = builder.BuildSelectClause(report.SelectedColumns);
            return ($"[{baseTable}]", joinClause, selectClause);
        });
    }

    #endregion
}