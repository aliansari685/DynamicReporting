namespace DynamicReporting.Api.Application.Jobs;

public class ExportJob(IKeyedServiceProvider serviceProvider) : IExportJob
{
    public async Task ExportToExcelJobAsync(int reportDefinitionId, CancellationToken cancellationToken = default)
    {
        var fileName = $"report_{Guid.NewGuid()}.xlsx";

        //todo: اگه فضای ابری باشه خطا میده کد پایین
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
        Directory.CreateDirectory(directory);

        var fullPath = Path.Combine(directory, fileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 2 * 1024 * 1024, useAsync: true);

        var excelService = ReportExportGetService("excel");
        await excelService.ExportAsync(reportDefinitionId, fileStream, cancellationToken);
    }

    public IReportExportService ReportExportGetService(string type)
    {
        return serviceProvider.GetRequiredKeyedService<IReportExportService>(type);
    }
}