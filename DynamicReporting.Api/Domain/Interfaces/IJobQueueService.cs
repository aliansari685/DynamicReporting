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

    /// <summary>
    /// تغییر وضعیت جاب به دیلیت
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Delete(string id);

    /// <summary>
    /// دریافت وضعیت حاب
    /// </summary>
    /// <param name="id">شناسه جاب</param>
    /// <returns></returns>
    public string GetStatusByJobId(int id);
}