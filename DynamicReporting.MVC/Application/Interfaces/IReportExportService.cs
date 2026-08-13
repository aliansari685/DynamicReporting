namespace DynamicReporting.Mvc.Application.Interfaces;

public interface IReportExportService
{
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