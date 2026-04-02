namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
/// اجرای بکگراند جاب ها با پکیج هنگ فایر
/// </summary>
/// <param name="backgroundJobClient">سرویس هنگ فایر</param>
public class HangfireJobQueueService(IBackgroundJobClient backgroundJobClient) : IJobQueueService
{
    public enum HangfireJobState
    {
        Enqueued,
        Processing,
        Succeeded,
        Failed,
        Scheduled,
        Deleted,
        Awaiting,
        AwaitingContinuation
    }
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string ContinueJob<T>(int jobId, Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.ContinueJobWith(jobId.ToString(), methodCall);
    }

    public bool Delete(int id)
    {
        return backgroundJobClient.Delete(id.ToString());
    }

    public DateTime GetExpireDateTimeByJobId(int id)
    {
        var monitor = JobStorage.Current.GetMonitoringApi();
        var details = monitor.JobDetails(id.ToString());
        return details.ExpireAt ?? throw new NullReferenceException("زمان منقضی شدن نا معتبر است");
    }

    public string GetStatusByJobId(int id)
    {
        using var connection = JobStorage.Current.GetConnection();
        var job = connection.GetJobData(id.ToString());
        return job.State ?? nameof(HangfireJobState.Deleted);
    }
    public string GetStatusByJobId1(int id)
    {
        var monitor = JobStorage.Current.GetMonitoringApi();
        var details = monitor.JobDetails(id.ToString());
        return details.History
            .OrderByDescending(h => h.CreatedAt)
            .First()
            .StateName;
    }
    public string GetStatusByJobId2(int id)
    {
        var monitor = JobStorage.Current.GetMonitoringApi();
        var details = monitor.JobDetails(id.ToString());
        return details.History.Last().StateName;
    }
}