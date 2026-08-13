namespace DynamicReporting.Mvc.Application.Interfaces;

public interface IReportGeneratedService
{
    Task<ReportGenerationVm> GetGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportGenerationVm>> GetGeneratedReportsAsync(
        CancellationToken cancellationToken = default);

    Task<string> GetGeneratedReportStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DownloadGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteGeneratedReportAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}