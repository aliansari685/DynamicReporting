using DynamicReporting.Api.Domain.Enums;

namespace DynamicReporting.Api.Application.Jobs;

/// <summary>
///     کلاس کران جاب- تسک زمان بندی شده
/// </summary>
public class CronJobs(IReportGeneratedService generatedService, IJobQueueService jobQueueService)
{
    /// <summary>
    ///     متد حذف فایل گزارش های منقضی شده
    /// </summary>
    /// <returns></returns>
    public async Task CleanupExpiredReportsJobAsync()
    {
        var reports = await generatedService.GetAllToListAsync();

        var expiredReports = reports
            .Where(x => x.ExpDateTime <= DateTime.UtcNow &&
                        !string.IsNullOrEmpty(x.DownloadUrl)).ToList();
        foreach (var report in expiredReports)
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

    /// <summary>
    ///     تغییر وضعیت جاب هایی ک به هردلیلی اجرا و انجام نشده
    /// </summary>
    public async Task CleanupFailedJobsAsync()
    {
        var generationResponseDto = await generatedService.GetAllToListAsync();
        foreach (var responseDto in generationResponseDto)
            try
            {
                var status = jobQueueService.GetStatusByJobId(responseDto.JobId);

                if (status != nameof(HangfireJobState.Succeeded) &&
                    status != nameof(HangfireJobState.Deleted))
                {
                    jobQueueService.Delete(responseDto.JobId);
                    Log.Error("Deleted JobId {JobId}", responseDto.JobId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in Changed Status for JobId {JobId}", responseDto.JobId);
            }
    }
}