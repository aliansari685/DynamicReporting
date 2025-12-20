namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(ShopTestDbContext dbContext, ISqlQueryExecutor sqlExecutor, IReportQueryBuilder queryBuilder) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>>
        GetReportDataAsync(int reportDefinitionId, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await dbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        if (report == null)
            throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");


        //کوئری تعداد
        var countSql = queryBuilder.BuildCountQuery(report);
        var totalCount = await sqlExecutor.ExecuteScalarAsync(countSql);

        // کوئری داده
        var dataSql = queryBuilder.BuildQuery(report, page, take);
        var data = await sqlExecutor.ExecuteAsync(dataSql);

        return new PagedResult<Dictionary<string, object?>>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            PageSize = take,
        };
    }

}