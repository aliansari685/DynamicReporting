namespace DynamicReporting.Api.Domain.Interfaces;

/// <summary>
/// قرارداد مرتبط به کتابخانه های مدیریت صف و جاب ها
/// </summary>
public interface IJobQueueService
{
    /// <summary>
    /// اجرای متد در بکگراند
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodCall"></param>
    string Enqueue<T>(Expression<Action<T>> methodCall);
}