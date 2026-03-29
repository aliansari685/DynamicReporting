namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    public Task<string> ExportInBackground(int reportDefinitionId, string type,
        CancellationToken cancellationToken = default);

}