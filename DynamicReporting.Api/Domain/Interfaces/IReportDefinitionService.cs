namespace DynamicReporting.Api.Domain.Interfaces;

/// <summary>
/// سرویس مدیریت قالب‌های گزارش (ReportDefinition) را ارائه می‌دهد.
/// این سرویس تمام عملیات CRUD و تنظیم قالب پیش‌فرض را فراهم می‌کند.
/// </summary>
public interface IReportDefinitionService
{
    /// <summary>
    /// قالب گزارش جدیدی ایجاد می‌کند.
    /// </summary>
    /// <param name="definition">شیء ReportDefinition که باید ایجاد شود.</param>
    Task CreateAsync(ReportDefinitionDto definition);

    /// <summary>
    /// قالب گزارشی با شناسه مشخص را برمی‌گرداند.
    /// </summary>
    /// <param name="id">شناسه قالب گزارش.</param>
    /// <returns>شیء ReportDefinition مربوط به شناسه داده شده.</returns>
    Task<ReportDefinition> GetByIdAsync(int id);


    /// <summary>
    /// دریافت ردیف با پراپرتی دلخواه
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<ReportDefinition?> GetByPropertyAsync(Expression<Func<ReportDefinition, bool>> predicate);

    /// <summary>
    /// همه قالب‌های گزارش موجود را برمی‌گرداند.
    /// </summary>
    /// <returns>یک مجموعه از تمام ReportDefinitionها.</returns>
    IEnumerable<ReportDefinition> GetAll();


    /// <summary>
    /// همه قالب‌های گزارش موجود را برمی‌گرداند.
    /// </summary>
    /// <returns>یک لیست از تمام ReportDefinitionها.</returns>
    Task<List<ReportDefinition>> GetAllToListAsync();

    /// <summary>
    /// قالب گزارش موجود را به‌روزرسانی می‌کند.
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <param name="definition">شیء ReportDefinition با مقادیر به‌روزشده.</param>
    Task UpdateAsync(int id, ReportDefinitionDto definition);

    /// <summary>
    /// قالب گزارش مشخصی را حذف می‌کند.
    /// </summary>
    /// <param name="id">شناسه قالب گزارش که باید حذف شود.</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// قالب گزارش مشخصی را به عنوان قالب پیش‌فرض تنظیم می‌کند.
    /// </summary>
    /// <param name="id">شناسه قالب گزارش که باید پیش‌فرض شود.</param>
    Task SetDefaultAsync(int id);


}

