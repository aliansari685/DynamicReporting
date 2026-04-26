namespace DynamicReporting.Api.Application.Jobs;

/// <summary>
/// کلاس کران جاب- تسک زمان بندی شده
/// </summary>
/// <param name="generatedService"></param>
public class CronJobs(IReportGeneratedService generatedService)
{
    /// <summary>
    ///  متد حذف فایل گزارش های منقضی شده
    /// </summary>
    /// <returns></returns>
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
                var fullPath = $"{Directory.GetCurrentDirectory()}\\{report.DownloadUrl}";
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