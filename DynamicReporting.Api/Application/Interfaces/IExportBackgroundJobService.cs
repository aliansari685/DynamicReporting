namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    public string ExportInBackground(int reportDefinitionId, string type, CancellationToken cancellationToken = default);

}