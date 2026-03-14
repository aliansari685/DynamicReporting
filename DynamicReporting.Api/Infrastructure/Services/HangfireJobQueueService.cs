namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
/// اجرای بکگراند جاب ها با پکیج هنگ فایر
/// </summary>
/// <param name="backgroundJobClient">سرویس هنگ فایر</param>
public class HangfireJobQueueService(IBackgroundJobClient backgroundJobClient) : IJobQueueService
{
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }
}