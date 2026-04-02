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
    /// تکمیل کردن جاب قبلی  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jobId">جاب ایدی قبلی</param>
    /// <param name="methodCall"></param>
    /// <returns></returns>
    public string ContinueJob<T>(int jobId, Expression<Action<T>> methodCall);

    /// <summary>
    /// تغییر وضعیت جاب به دیلیت
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Delete(int id);

    /// <summary>
    /// دریافت وضعیت حاب
    /// </summary>
    /// <param name="id">شناسه جاب</param>
    /// <returns></returns>
    public string GetStatusByJobId(int id);

    /// <summary>
    /// دریافت زمان منقضی شدن جاب
    /// </summary>
    /// <param name="id">جاب ایدی</param>
    /// <returns></returns>
    public DateTime GetExpireDateTimeByJobId(int id);
}