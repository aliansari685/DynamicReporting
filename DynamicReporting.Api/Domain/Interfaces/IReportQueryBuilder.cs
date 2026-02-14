namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportQueryBuilder
{
    /// <summary>
    /// الگوریتم ساخت کوئری داینامیک با پیدا کردن جدول پایه - سپس پیدا کردن فارن کی ها - سپس ایجاد جوین و رابطه بین جدوال با توجه ب الگوریتم گراف
    /// یعنی پیدا کردن نزدیک ترین فارن کی جهت ایجاد جوین(مسیر ارتباط)
    /// </summary>
    /// <param name="report"> ردیف قالب رپیورت</param>
    /// <param name="offset">تعداد ردیف</param>
    /// <param name="take">تعداد ردیف برای گذشتن</param>
    /// <returns>کوئری اماده</returns>
    string BuildQuery(ReportDefinition report, int offset, int take);


    /// <summary>
    ///استفاده از متد بیلد کوئری و اضافه کردن دستورات مربوط به صفحه بندی
    /// </summary>
    /// <param name="report"></param>
    /// <param name="page"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    string BuildPagedQuery(ReportDefinition report, int page, int take);

    /// <summary>
    /// ساخت کوئری کانت و تعداد دیتا
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    string BuildCountQuery(ReportDefinition report);
}