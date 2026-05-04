namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    /// <summary>
    /// ساخت خروجی از گزارش در بکگراند 
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <param name="filtersList"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Guid> ExportInBackground(int reportDefinitionId, List<FilterCondition>? filtersList,
        ServiceResolver.ExportType type,
        CancellationToken cancellationToken = default);

}