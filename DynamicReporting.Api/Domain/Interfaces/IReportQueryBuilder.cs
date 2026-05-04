namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportQueryBuilder
{
    ///  <summary>
    /// استفاده از متد بیلد کوئری و اضافه کردن دستورات مربوط به صفحه بندی
    ///  </summary>
    ///  <param name="report"></param>
    ///  <param name="whereClause"></param>
    ///  <param name="page"></param>
    ///  <param name="take"></param>
    ///  <returns></returns>
    string BuildPagedQuery(ReportDefinition report, string whereClause, int page, int take);

    /// <summary>
    /// ساخت کوئری کانت و تعداد دیتا
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    string BuildCountQuery(ReportDefinition report);

    /// <summary>
    /// ساخت کوئری شرط ها برای اعمال فیلتر
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public (string whereClause, Dictionary<string, object> parameters) BuildWhereClause(List<FilterCondition>? filters);

    /// <summary>
    /// الگوریتم ساخت کوئری داینامیک با پیدا کردن جدول پایه - سپس پیدا کردن فارن کی ها - سپس ایجاد جوین و رابطه بین جدوال با توجه ب الگوریتم گراف
    /// یعنی پیدا کردن نزدیک ترین فارن کی جهت ایجاد جوین(مسیر ارتباط)
    /// </summary>
    /// <param name="report"> ردیف قالب رپیورت</param>
    /// <param name="whereClause"></param>
    /// <param name="offset">تعداد ردیف</param>
    /// <param name="take">تعداد ردیف برای گذشتن</param>
    /// <returns>کوئری اماده</returns>
    public string BuildQuery(ReportDefinition report, string whereClause, int offset, int take);
}