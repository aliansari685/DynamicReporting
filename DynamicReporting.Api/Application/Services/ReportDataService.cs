namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(IServiceResolver serviceProvider, IReportQueryBuilder reportQueryBuilder, IMemoryCache memoryCache, IUnitOfWork uow) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, SortableColumnDto sortColumn, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await GetReportDefinitionAsync(reportDefinitionId);

        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

        var dataSql = reportQueryBuilder.BuildPagedQuery(report, whereClause, page, take, sortColumn);

        var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

        var data = await executorService.ExecuteAsync(dataSql, parameters);

        var result = GetDisplayNameColumn(data);

        data = result;

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId);

        var pagedResult = new PagedResult<Dictionary<string, object?>>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            Take = take,
            SortBy = sortColumn.Column,
            Dir = sortColumn.SortDirection.ToString()
        };

        return pagedResult;
    }
    public async Task<int> GetTotalCountAsync(int reportDefinitionId, ReportDefinition? definition = null)
    {
        var report = definition ?? await GetReportDefinitionAsync(reportDefinitionId);

        var cacheKey = $"report:{reportDefinitionId}:count";

        var totalCount = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var countSql = reportQueryBuilder.BuildCountQuery(report);

            var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

            return await executorService.ExecuteScalarAsync(countSql);
        });

        return totalCount;
    }

    #region Helper Methods
    private async Task<ReportDefinition> GetReportDefinitionAsync(int reportDefinitionId)
    {
        var report = await uow.DbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }

    private List<Dictionary<string, object?>> GetDisplayNameColumn(List<Dictionary<string, object?>> data)
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

    #endregion
}