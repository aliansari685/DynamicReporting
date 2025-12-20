namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportQueryBuilder
{
    /// <summary>
    /// الگوریتم ساخت کوئری داینامیک با پیدا کردن جدول پایه - سپس پیدا کردن فارن کی ها - سپس ایجاد جوین و رابطه بین جدوال با توجه ب الگوریتم گراف
    /// یعنی پیدا کردن نزدیک ترین فارن کی جهت ایجاد جوین(مسیر ارتباط)
    /// </summary>
    /// <param name="report"> ردیف قالب رپیورت</param>
    /// <param name="page">صفحه</param>
    /// <param name="take"></param>
    /// <returns>کوئری اماده</returns>
    string BuildQuery(ReportDefinition report, int page, int take);


    /// <summary>
    /// ساخت کوئری کانت و تعداد دیتا
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    string BuildCountQuery(ReportDefinition report);
}