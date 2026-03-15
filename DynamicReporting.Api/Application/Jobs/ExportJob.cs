namespace DynamicReporting.Api.Application.Jobs;

public class ExportJob(IReportExportServiceResolver serviceResolver) : IExportJob
{
    public async Task ExportJobAsync(int reportDefinitionId, string type, CancellationToken cancellationToken = default)
    {
        var fullPath = CreateExportFile(type);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 2 * 1024 * 1024, useAsync: true);

        var exportService = serviceResolver.GetService(type);
        await exportService.ExportAsync(reportDefinitionId, fileStream, cancellationToken);
    }

    private string CreateExportFile(string type)
    {
        string fileName = $"report_{Guid.NewGuid()}";

        fileName += type.ToLower() switch
        {
            "excel" => ".xlsx",
            "pdf" => ".pdf",
            _ => throw new ArgumentOutOfRangeException(type, "ورودی فایل وجود ندارد")
        };

        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }
}