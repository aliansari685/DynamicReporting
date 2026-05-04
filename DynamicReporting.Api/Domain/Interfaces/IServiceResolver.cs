namespace DynamicReporting.Api.Domain.Interfaces;

public interface IServiceResolver
{
    /// <summary>
    /// بدست اوردن سرویس موردنظر برای خروجی
    /// </summary>
    /// <param name="type">نوع خروجی مثل پیدیاف و اکسل</param>
    /// <returns></returns>
    public IReportExportService GetExportService(ServiceResolver.ExportType type);

    /// <summary>
    /// بدست اوردن سرویس برای اجرای کوئری ها
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public ISqlQueryExecutor GetExecutorService(ServiceResolver.ExecutorType type);
}