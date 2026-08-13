namespace DynamicReporting.Api.Domain.Enums;

/// <summary>
///     وضعیت های رسمی جاب در هنگ فایر
/// </summary>
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