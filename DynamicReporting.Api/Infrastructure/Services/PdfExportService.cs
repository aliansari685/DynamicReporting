namespace DynamicReporting.Api.Infrastructure.Services;

//todo :
public class PdfExportService(
    IReportDataService reportDataService,
    IReportExportService exportService,
    IReportMetadataService metadataService,
    IReportQueryBuilder reportQueryBuilder) : IExportService
{
    public Task<bool> ExportAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream,
        SortableColumnDto sortColumn, CancellationToken cancellationToken = default)
    {
        {
            return Task.FromResult(true);
        }
    }

    public Task ExportWithAutoFitColumnsAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream,
        SortableColumnDto sortColumn, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}