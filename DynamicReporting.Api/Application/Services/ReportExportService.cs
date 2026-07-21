namespace DynamicReporting.Api.Application.Services;

public class ReportExportService(
    IServiceResolver serviceProvider,
    IReportQueryBuilder reportQueryBuilder,
    IUnitOfWork uow) : IReportExportService
{
    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList,
        int offset, int take, SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await GetReportDefinitionAsync(reportDefinitionId);

        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

        var sql = reportQueryBuilder.BuildQuery(report, whereClause, offset, take, sortColumn);

        var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

        return await executorService.ExecuteAsync(sql, parameters, cancellationToken);
    }

    #region Helper Methods

    private async Task<ReportDefinition> GetReportDefinitionAsync(int reportDefinitionId)
    {
        var report = await uow.DbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }

    #endregion
}