namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(IServiceResolver serviceProvider, IReportQueryBuilder reportQueryBuilder, IMemoryCache memoryCache, ISelectJoinBuilder builder, IUnitOfWork uow) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await GetReportDefinitionAsync(reportDefinitionId);

        //todo : با فیلتر تست کن و مثال از یکی فیلتر ساده بزار
        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

        var dataSql = reportQueryBuilder.BuildPagedQuery(report, whereClause, page, take);

        var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

        var data = await executorService.ExecuteAsync(dataSql, parameters);

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId);

        PagedResult<Dictionary<string, object?>> pagedResult;


        //if (filtersList == null || filtersList.Count == 0)
        //{
        //    pagedResult = new PagedResult<Dictionary<string, object?>>
        //    {
        //        Data = data,
        //        TotalCount = 200,
        //        Page = page,
        //        Take = take,
        //    };
        //}
        //else
        {
            pagedResult = new PagedResult<Dictionary<string, object?>>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                Take = take,
            };
        }

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

    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId, List<FilterCondition>? filtersList, int offset, int take, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await GetReportDefinitionAsync(reportDefinitionId);

        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

        var sql = reportQueryBuilder.BuildQuery(report, whereClause, offset, take);

        var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

        return await executorService.ExecuteAsync(sql, parameters, cancellationToken);
    }

    /// <summary>`
    /// بدست اوردن ReportDefinition از دیتابیس با شناسه داده شده
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private async Task<ReportDefinition> GetReportDefinitionAsync(int reportDefinitionId)
    {
        var report = await uow.DbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }

    public async Task<List<string>> GetFilterableColumnsAsync(int reportDefinitionId)
    {
        var reportDefineEntity = await GetReportDefinitionAsync(reportDefinitionId);

        var tables = reportDefineEntity.SelectedColumns
            .Select(c => c.Table)
            .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToList();

        return tables;

        //var res = builder.BuildJoinClause(reportDefineEntity.BaseTable, reportDefineEntity.SelectedColumns, uow.GetTrustEntityType);
    }


    private List<string> GetSupportedOperators(string dataType)
    {
        return dataType.ToLower() switch
        {
            "int" or "bigint" or "smallint" or "tinyint" or
                "decimal" or "money" or "float" or "real" =>
                ["eq", "gt", "gte", "lt", "lte"],

            "nvarchar" or "varchar" or "char" or "nchar" or "text" =>
                ["eq", "contains", "startswith", "endswith"],

            "datetime" or "datetime2" or "date" or "time" =>
                ["eq", "gt", "gte", "lt", "lte"],

            _ => ["eq"]
        };
    }
}