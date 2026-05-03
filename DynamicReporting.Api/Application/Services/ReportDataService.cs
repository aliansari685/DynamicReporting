namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(ShopTestDbContext dbContext, ISqlQueryExecutor sqlExecutor, IReportQueryBuilder reportQueryBuilder, IMemoryCache memoryCache) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await GetReportDefinition(reportDefinitionId);

        // کوئری داده برای پجینیشن
        var dataSql = reportQueryBuilder.BuildPagedQuery(report, filtersList, page, take);
        var data = await sqlExecutor.ExecuteAsync(dataSql);

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId);

        var pagedResult = new PagedResult<Dictionary<string, object?>>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            Take = take,
        };
        return pagedResult;
    }

    public async Task<int> GetTotalCountAsync(int reportDefinitionId, ReportDefinition? definition = null)
    {
        var report = definition ?? await GetReportDefinition(reportDefinitionId);

        var cacheKey = $"report:{reportDefinitionId}:count";

        var totalCount = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var countSql = reportQueryBuilder.BuildCountQuery(report);

            return await sqlExecutor.ExecuteScalarAsync(countSql);
        });

        return totalCount;
    }

    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId, List<FilterCondition>? filtersList, int offset, int take, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await GetReportDefinition(reportDefinitionId);
        var sql = reportQueryBuilder.BuildQuery(report, filtersList, offset, take);

        return await sqlExecutor.ExecuteAsync(sql, cancellationToken);
    }

    /// <summary>
    /// بدست اوردن ReportDefinition از دیتابیس با شناسه داده شده
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private async Task<ReportDefinition> GetReportDefinition(int reportDefinitionId)
    {
        var report = await dbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }
}