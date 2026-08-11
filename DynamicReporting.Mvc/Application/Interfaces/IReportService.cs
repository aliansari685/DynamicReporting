namespace DynamicReporting.Mvc.Services;

public interface IReportService
{
    Task<IReadOnlyList<ReportDefinitionVm>> GetReportsAsync(
        CancellationToken cancellationToken = default);

    Task<ReportDefinitionVm?> GetDefaultReportAsync(
        CancellationToken cancellationToken = default);

    Task<PagedReportDataVm> GetReportDataAsync(
        int reportDefinitionId,
        string? filters,
        string? sort,
        string? dir,
        int page,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportColumnVm>> GetFilterableColumnsAsync(
        int reportDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportColumnVm>> GetSortableColumnsAsync(
        int reportDefinitionId,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> FastExportAsync(
        int reportDefinitionId,
        string? filters,
        string? sort,
        string? dir,
        CancellationToken cancellationToken = default);

    Task<ExportJobResultVm> ExportAsync(
        int reportDefinitionId,
        string? filters,
        string? sort,
        string? dir,
        string type,
        CancellationToken cancellationToken = default);
}