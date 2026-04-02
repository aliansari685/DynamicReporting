namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    public Task<Guid> ExportInBackground(int reportDefinitionId, string type,
        CancellationToken cancellationToken = default);

}