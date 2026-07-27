namespace DynamicReporting.Api.Application.Services;

public class ReportExportService(
    IServiceResolver serviceProvider,
    IReportQueryBuilder reportQueryBuilder,
    IReportValidation reportValidation,
    IReportMetadataService metadataService) : IReportExportService
{
    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, int offset, int take, SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

        if (filtersList != null)
            reportValidation.ValidateFilteringColumn(report, filtersList);

        var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);
        reportValidation.ValidateSortColumn(report, sortColumn);
        var sql = reportQueryBuilder.BuildQuery(report, whereClause, offset, take, sortColumn);
        var executorService = serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);
        return await executorService.ExecuteAsync(sql, parameters, cancellationToken);
    }
}