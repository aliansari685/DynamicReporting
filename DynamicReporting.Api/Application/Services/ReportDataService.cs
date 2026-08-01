namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(
    IServiceResolver serviceProvider,
    IReportQueryBuilder reportQueryBuilder,
    IMemoryCache memoryCache,
    IReportMetadataService metadataService,
    IReportValidation reportValidation)
    : IReportDataService
{
    //todo : Here you can switch between the sql execution engine, which is Ado.Net or Dapper.
    private readonly ISqlQueryExecutor _executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.Dapper);

    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, SortableColumnDto sortColumn, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

        if (filtersList != null) reportValidation.ValidateFilteringColumn(report, filtersList);
        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

        reportValidation.ValidateSortColumn(report, sortColumn);

        var dataSql = reportQueryBuilder.BuildPagedQuery(report, whereClause, page, take, sortColumn);

        var data = await _executorService.ExecuteAsync(dataSql, parameters);

        var result = metadataService.GetDisplayNameColumn(data);

        data = result;

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId, (whereClause, parameters));

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

    public async Task<int> GetTotalCountAsync(int reportDefinitionId,
        (string whereClause, Dictionary<string, object> parameters) tuple)
    {
        var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

        var parametersPart = string.Join("&", tuple.parameters
            .Select(p => $"{p.Key}={p.Value.ToString() ?? "null"}")
            .OrderBy(p => p)); // مرتب‌سازی برای یکنواختی

        var cacheKey = $"report{reportDefinitionId}|filters:{tuple.whereClause}|params:{parametersPart}|count";

        var totalCount = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);

            var countSql = reportQueryBuilder.BuildCountQuery(report, tuple.whereClause);

            return await _executorService.ExecuteScalarAsync(countSql, tuple.parameters);
        });

        return totalCount;
    }
}