namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

public sealed class EfReportQueryBuilder(ShopTestDbContext dbContext, ISelectJoinBuilder builder, IQueryCacheManager cacheManager) : IReportQueryBuilder
{
    public string BuildCountQuery(ReportDefinition report)
    {
        var template = GetQueryTemplate(report);

        return $"""
                SELECT COUNT(1)
                FROM
                (
                    SELECT 1 AS x
                    FROM {template.FromClause}
                    {template.JoinClause}
                ) AS q
                """;
    }

    public string BuildQuery(ReportDefinition report, int offset, int take)
    {
        var template = GetQueryTemplate(report);

        return $"""
                SELECT
                {template.SelectClause}
                FROM {template.FromClause}
                {template.JoinClause}
                ORDER BY (SELECT NULL)
                OFFSET {offset} ROWS
                FETCH NEXT {take} ROWS ONLY
                """;
    }

    public string BuildPagedQuery(ReportDefinition report, int page, int take)
    {
        var offset = (page - 1) * take;
        return BuildQuery(report, offset, take);
    }

    /// <summary>
    /// بازگردانی template query برای یک گزارش شامل FROM, JOIN و SELECT clauses.
    /// - ابتدا بررسی می‌کند که template از cache موجود باشد.
    /// - اگر موجود نباشد، factory فراخوانی شده و template محاسبه و در cache ذخیره می‌شود.
    /// - هدف: جلوگیری از محاسبات تکراری Join و Select در pagination و گزارش‌های بزرگ.
    /// - از JoinPathResolver و SelectJoinBuilder برای محاسبه join و select استفاده می‌شود.
    /// </summary>
    /// <param name="report">گزارش مورد نظر که شامل BaseTable و SelectedColumns است</param>
    /// <returns>
    /// Tuple شامل:
    /// - FromClause: جدول پایه با کروشه SQL `[TableName]`
    /// - JoinClause: رشته JOIN بین جدول پایه و جداول مرتبط
    /// - SelectClause: رشته SELECT شامل ستون‌های انتخاب‌شده با alias
    /// </returns>
    private (string FromClause, string JoinClause, string SelectClause) GetQueryTemplate(ReportDefinition report)
    {
        return cacheManager.GetOrCreate(report.Id, () =>
        {
            var baseTable = report.BaseTable;
            var joinClause = builder.BuildJoinClause(baseTable, report.SelectedColumns!, GetEntityType);
            var selectClause = builder.BuildSelectClause(report.SelectedColumns!);
            return ($"[{baseTable}]", joinClause, selectClause);
        });
    }


    /// <summary>
    /// دریافت EntityType مربوط به نام جدول دیتابیس از EF Core Model.
    /// - فقط metadata را جستجو می‌کند، نه داده‌های واقعی.
    /// - برای استفاده در JoinPathResolver و SelectJoinBuilder جهت محاسبه مسیر Join و ساخت query.
    /// - فرض: نام جدول دقیقا با نام EF EntityType مطابقت دارد (case-insensitive)
    /// </summary>
    /// <param name="tableName">نام جدول دیتابیس</param>
    /// <returns>IEntityType متناظر با جدول</returns>
    /// <exception cref="InvalidOperationException">اگر جدول مورد نظر در EF Model پیدا نشود.</exception>
    private IEntityType GetEntityType(string tableName)
    {
        return dbContext.Model.GetEntityTypes()
            .First(e => e.GetTableName()!.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }

}