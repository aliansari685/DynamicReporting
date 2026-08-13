namespace DynamicReporting.Mvc.Application.Interfaces;

public interface IReportDefinitionService
{
    Task<ReportDefinitionVm?> GetReportAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task CreateReportAsync(
        ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default);

    Task UpdateReportAsync(
        int id,
        ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteReportAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportDefinitionVm>> GetReportsAsync(
        CancellationToken cancellationToken = default);

   Task<ReportDefinitionVm?> GetDefaultReportAsync(
        CancellationToken cancellationToken = default);
}