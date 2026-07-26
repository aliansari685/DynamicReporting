namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportQueryBuilder
{
    /// <summary>
    ///     استفاده از متد بیلد کوئری و اضافه کردن دستورات مربوط به صفحه بندی
    /// </summary>
    /// <param name="report">کدام گزارش</param>
    /// <param name="whereClause">شرط ها برای اعمال فیلتر</param>
    /// <param name="page">صفحه ی چند؟</param>
    /// <param name="take">تعداد ردیف هر صفحه</param>
    /// <param name="sortColumn">مرتب سازی گزارش بر اساس فلان ستون</param>
    /// <returns></returns>
    string BuildPagedQuery(ReportDefinition report, string whereClause, int page, int take,
        SortableColumnDto sortColumn);

    /// <summary>
    ///     ساخت کوئری کانت و تعداد دیتا
    /// </summary>
    /// <param name="report"></param>
    /// <param name="whereClause">شرط ها برای اعمال فیلتر</param>
    /// <returns></returns>
    public string BuildCountQuery(ReportDefinition report, string whereClause);

    /// <summary>
    ///     ساخت کوئری شرط ها برای اعمال فیلتر
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public (string whereClause, Dictionary<string, object> parameters) BuildWhereClause(List<FilterCondition>? filters);

    /// <summary>
    ///     الگوریتم ساخت کوئری داینامیک با پیدا کردن جدول پایه - سپس پیدا کردن فارن کی ها - سپس ایجاد جوین و رابطه بین جدوال
    ///     با توجه ب الگوریتم گراف
    ///     یعنی پیدا کردن نزدیک ترین فارن کی جهت ایجاد جوین(مسیر ارتباط)
    /// </summary>
    /// <param name="report"> ردیف قالب رپیورت</param>
    /// <param name="whereClause"></param>
    /// <param name="offset">تعداد ردیف</param>
    /// <param name="take">تعداد ردیف برای گذشتن</param>
    /// <param name="sortColumn">ستونی ک قراره مرتب سازی انجام بشه روش ، پیش فرض کلید اصلی جدول</param>
    /// <returns>کوئری اماده</returns>
    public string BuildQuery(ReportDefinition report, string whereClause, int offset, int take,
        SortableColumnDto sortColumn);

}