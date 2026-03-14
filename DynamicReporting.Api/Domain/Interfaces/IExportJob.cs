namespace DynamicReporting.Api.Domain.Interfaces;

public interface IExportJob
{
    public Task ExportToExcelJobAsync(int reportDefinitionId, CancellationToken cancellationToken = default);
}