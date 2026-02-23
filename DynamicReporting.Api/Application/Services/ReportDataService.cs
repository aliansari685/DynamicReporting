using System.Diagnostics;

namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(ShopTestDbContext dbContext, ISqlQueryExecutor sqlExecutor, IReportQueryBuilder queryBuilder, IMemoryCache memoryCache) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        Program.Stopwatch1.Restart();

        var report = await GetReportDefinition(reportDefinitionId);

        Program.Stopwatch1.Stop();
        Log.Error("GetReportDefinition:" + Program.Stopwatch1.ElapsedMilliseconds);

        Program.Stopwatch1.Restart();

        // کوئری داده برای پجینیشن
        var dataSql = queryBuilder.BuildPagedQuery(report, page, take);
        var data = await sqlExecutor.ExecuteAsync(dataSql);

        Program.Stopwatch1.Stop();
        Log.Error("ExeDataSql:" + Program.Stopwatch1.ElapsedMilliseconds);

        Program.Stopwatch1.Restart();

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId);

        var pagedResult = new PagedResult<Dictionary<string, object?>>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            Take = take,
        };
        Program.Stopwatch1.Stop();
        Log.Error("pagedResult:" + Program.Stopwatch1.ElapsedMilliseconds);

        return pagedResult;
    }

    public async Task<int> GetTotalCountAsync(int reportDefinitionId, ReportDefinition? definition = null)
    {
        return await TimeLogger.TimeAsync(async () =>
            {
                var report = definition ?? await GetReportDefinition(reportDefinitionId);

                var cacheKey = $"report:{reportDefinitionId}:count";

                var totalCount = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                    string countSql = "";

                    TimeLogger.Time(() =>
                 countSql = queryBuilder.BuildCountQuery(report), "BuildCountQuery:");

                    int tmp = 0;

                    await TimeLogger.TimeAsync(async () =>
                        tmp = await sqlExecutor.ExecuteScalarAsync(countSql), "ExecuteScalarAsync:");

                    return tmp;
                });

                return totalCount;

            }, $"GetTotalCountAsync({reportDefinitionId})");
    }

    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId, int offset, int take)
    {
        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await GetReportDefinition(reportDefinitionId);
        var sql = queryBuilder.BuildQuery(report, offset, take);

        var batch = await sqlExecutor.ExecuteAsync(sql);
        return batch;
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