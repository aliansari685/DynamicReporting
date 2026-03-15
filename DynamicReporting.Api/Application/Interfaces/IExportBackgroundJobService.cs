namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    public string ExportToExcelInBackground(int reportDefinitionId, CancellationToken cancellationToken = default);

}