namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    /// <summary>
    /// ساخت خروجی از گزارش در بکگراند 
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Guid> ExportInBackground(int reportDefinitionId, string type,
        CancellationToken cancellationToken = default);

}