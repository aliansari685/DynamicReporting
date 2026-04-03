namespace DynamicReporting.Api.Application.Jobs;

public class CronJobs(IReportGeneratedService generatedService)
{
    public async Task CleanupExpiredReportsJob()
    {
        var reports = await generatedService.GetAllToListAsync();

        var expiredReports = reports
            .Where(x => x.ExpDateTime <= DateTime.UtcNow &&
                                                !string.IsNullOrEmpty(x.DownloadUrl)).ToList();
        foreach (var report in expiredReports)
        {
            try
            {
                var fullPath = Directory.GetCurrentDirectory() + report.DownloadUrl;
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Log.Information("Deleted file: {Path}", report.DownloadUrl);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting file: {Path}", report.DownloadUrl);
            }
        }
    }
}