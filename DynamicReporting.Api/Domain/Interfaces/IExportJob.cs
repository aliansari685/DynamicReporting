namespace DynamicReporting.Api.Domain.Interfaces;

public interface IExportJob
{
    /// <summary>
    /// جاب خروجی گرفتن
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ExportJobAsync(int reportDefinitionId, string type, CancellationToken cancellationToken = default);
}