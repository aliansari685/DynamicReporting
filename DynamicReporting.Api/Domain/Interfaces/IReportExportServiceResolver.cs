namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportExportServiceResolver
{
    /// <summary>
    /// بدست اوردن سرویس موردنظر از فکتوری سرویسمون
    /// </summary>
    /// <param name="type">نوع خروجی مثل پیدیاف و اکسل</param>
    /// <returns></returns>
    public IReportExportService GetService(string type);
}