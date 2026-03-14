namespace DynamicReporting.Api.Domain.Interfaces;

public interface IExportBackgroundJobService
{
    public string ExportToExcelInBackground(int reportDefinitionId, CancellationToken cancellationToken = default);

}