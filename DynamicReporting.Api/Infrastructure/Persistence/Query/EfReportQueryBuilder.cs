namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
///     مسئول ساخت Query داینامیک گزارش‌ها بر اساس
///     جدول پایه، ستون‌های انتخاب‌شده و روابط FK موجود در EF Model
/// </summary>
public sealed class EfReportQueryBuilder(ShopTestDbContext dbContext) : IReportQueryBuilder
{
    /// <summary>
    ///     کش مسیر Join بین دو جدول (برای جلوگیری از BFS تکراری)
    ///     کلید: (جدول مبدا، جدول مقصد)
    ///     مقدار: لیست FKهای مسیر Join
    /// </summary>
    private readonly ConcurrentDictionary<(string From, string To), List<IReadOnlyForeignKey>>
        _joinPathCache = new();

    /// <summary>
    ///     کش EntityTypeها بر اساس نام جدول دیتابیس
    ///     برای کاهش هزینه‌ی جستجو در Model
    /// </summary>
    private readonly ConcurrentDictionary<string, IEntityType>
        _entityTypeCache = new(StringComparer.OrdinalIgnoreCase);

    private string _baseTable = "";
    private string _joinClause = "";

    public string BuildCountQuery(ReportDefinition report)
    {
        if (report.SelectedColumns == null || report.SelectedColumns.Count == 0)
            throw new InvalidOperationException("هیچ ستونی برای گزارش انتخاب نشده است.");

        _baseTable = report.BaseTable;
        _joinClause = BuildDynamicJoins(_baseTable, report.SelectedColumns);

        return $"""
                SELECT COUNT(1)
                FROM [{_baseTable}]
                {_joinClause}
                """;
    }


    public string BuildQuery(ReportDefinition report, int page, int take)
    {
        var selectClause = BuildSelectClause(report.SelectedColumns!);

        var offset = CalculateOffset(page, take);

        return $"""
                SELECT
                {selectClause}
                FROM [{_baseTable}]
                {_joinClause}
                ORDER BY (SELECT NULL)
                OFFSET {offset} ROWS
                FETCH NEXT {take} ROWS ONLY
                """;
    }

    #region SELECT

    /// <summary>
    ///     تولید بخش سلکت بر اساس ستون‌های انتخاب‌شده
    /// </summary>
    /// <param name="columns">لیست ستون‌ها</param>
    /// <returns>رشته‌ی SELECT SQL</returns>
    private string BuildSelectClause(IEnumerable<SelectedColumn> columns) =>
        string.Join(", ",
            columns.Select(c =>
                $"[{c.Table}].[{c.Column}] AS [{c.Table}_{c.Column}]"));

    #endregion

    #region JOIN

    /// <summary>
    ///     تولید جوین های داینامیک بین جدول پایه و جداول مورد نیاز
    ///     با استفاده از کوتاه‌ترین مسیر فارن کی
    /// </summary>
    /// <param name="baseTable">جدول پایه گزارش</param>
    /// <param name="columns">ستون‌های انتخاب‌شده</param>
    /// <returns>رشته‌ی Joinهای SQL</returns>
    private string BuildDynamicJoins(string baseTable, List<SelectedColumn> columns)
    {
        // جدول‌هایی که تا این لحظه به Query اضافه شده‌اند
        var joinedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            baseTable
        };

        var joins = new StringBuilder();

        // جدول‌هایی که باید به جدول پایه جوین شوند
        var targetTables = columns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(t => !t.Equals(baseTable, StringComparison.OrdinalIgnoreCase));

        foreach (var targetTable in targetTables)
        {
            if (joinedTables.Contains(targetTable))
                continue;

            var fromEntity = GetEntityType(baseTable);
            var toEntity = GetEntityType(targetTable);

            // دریافت یا محاسبه مسیر جوین بین دو جدول
            var joinPath = _joinPathCache.GetOrAdd(
                (fromEntity.GetTableName()!, toEntity.GetTableName()!),
                _ => FindJoinPath(fromEntity, toEntity));

            foreach (var fk in joinPath) AppendJoinIfNeeded(fk, joinedTables, joins);
        }

        return joins.ToString();
    }


    #region FK Graph

    /// <summary>
    ///     پیدا کردن کوتاه‌ترین مسیر Join بین دو Entity
    ///     با استفاده از BFS(Breadth-First Search) روی گراف FKها
    /// جستجوی سطح به سطح
    /// </summary>
    /// <param name="from">Entity مبدا</param>
    /// <param name="to">Entity مقصد</param>
    /// <returns>لیست FKهای مسیر Join</returns>
    /// <exception cref="InvalidOperationException">
    ///     در صورتی که هیچ مسیر FK بین دو جدول وجود نداشته باشد
    /// </exception>
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

        throw new InvalidOperationException(
            $"هیچ مسیر ارتباطی (FK) بین جدول‌های '{from.GetTableName()}' و '{to.GetTableName()}' وجود ندارد.");
    }

    #endregion

    /// <summary>
    ///     افزودن یک Join به Query در صورتی که جدول مقصد
    ///     قبلاً Join نشده باشد
    /// </summary>
    private void AppendJoinIfNeeded(
        IReadOnlyForeignKey fk,
        HashSet<string> joinedTables,
        StringBuilder joins)
    {
        var principalTable = fk.PrincipalEntityType.GetTableName()!;
        var dependentTable = fk.DeclaringEntityType.GetTableName()!;

        var principalKey = fk.PrincipalKey.Properties[0].GetColumnName();
        var foreignKey = fk.Properties[0].GetColumnName();

        var dependentJoined = joinedTables.Contains(dependentTable);
        var principalJoined = joinedTables.Contains(principalTable);

        if (!dependentJoined && !principalJoined)
            throw new InvalidOperationException("ترتیب مسیر Join نامعتبر است و امکان ساخت Join وجود ندارد.");

        var fromTable = dependentJoined ? dependentTable : principalTable;
        var fromColumn = dependentJoined ? foreignKey : principalKey;
        var toTable = dependentJoined ? principalTable : dependentTable;
        var toColumn = dependentJoined ? principalKey : foreignKey;

        if (joinedTables.Add(toTable))
            joins.AppendLine(
                $"LEFT JOIN [{toTable}] ON " +
                $"[{toTable}].[{toColumn}] = [{fromTable}].[{fromColumn}]");
    }

    #endregion

    #region Metadata

    /// <summary>
    ///     دریافت EntityType مربوط به یک نام جدول دیتابیس
    ///     با استفاده از EF Model و کش داخلی
    /// </summary>
    /// <param name="tableName">نام جدول دیتابیس</param>
    /// <returns>EntityType متناظر</returns>
    private IEntityType GetEntityType(string tableName) =>
        _entityTypeCache.GetOrAdd(tableName, t =>
            dbContext.Model.GetEntityTypes()
            .First(e => e.GetTableName()!
                .Equals(t, StringComparison.OrdinalIgnoreCase)));

    #endregion

    #region InsertOffsetForPagination
    /// <summary>
    /// «از بین همه داده‌ها،
    /// اول ۲۰ تای اول رو بی‌خیال شو،
    /// بعد فقط ۱۰ تای بعدی رو بده،
    /// بدون سورت »
    /// </summary>
    /// <param name="page">صفحه</param>
    /// <param name="take">تعداد ردیف</param>
    /// <returns></returns>
    private int CalculateOffset(int page, int take)
        => (page - 1) * take;

    #endregion

}