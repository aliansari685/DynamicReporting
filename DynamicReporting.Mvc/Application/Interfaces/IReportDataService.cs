namespace DynamicReporting.Mvc.Application.Interfaces;

public interface IReportDataService
{
    Task<PagedReportDataVm> GetReportDataAsync(
        int reportDefinitionId,
        string? filters = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int take = 10,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FilterableColumnVm>> GetFilterableColumnsAsync(
        int reportDefinitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SortableColumnVm>> GetSortableColumnsAsync(
        int reportDefinitionId,
        CancellationToken cancellationToken = default);
}
