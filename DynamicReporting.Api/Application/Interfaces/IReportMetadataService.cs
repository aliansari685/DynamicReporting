namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportMetadataService
{
    /// <summary>
    ///     دریافت نام تمام جدول‌های ثبت شده در کانتکست.
    /// </summary>
    /// <returns>لیستی از رشته‌ها که هر رشته نام یک جدول را نشان می‌دهد.</returns>
    List<DisplayMetadata> GetAllTableNames();

    /// <summary>
    ///     دریافت متادیتا تمامی جدول‌های ثبت شده در کانتکست.
    ///     هر جدول شامل نام جدول و لیست ستون‌هایش می‌باشد.
    /// </summary>
    /// <returns>لیستی از <see cref="TableMetadata" /> که هر عنصر نمایانگر یک جدول و ستون‌های آن است.</returns>
    List<TableMetadata> GetAllMetadata();

    /// <summary>
    ///     دریافت متادیتا یک جدول مشخص بر اساس نام جدول.
    ///     شامل نام جدول و لیست ستون‌های آن می‌باشد.
    /// </summary>
    /// <param name="tableName">نام جدول مورد نظر</param>
    /// <returns>یک <see cref="TableMetadata" /> که نام جدول و ستون‌های آن را شامل می‌شود.</returns>
    /// <exception cref="KeyNotFoundException">در صورتی که جدول مورد نظر در کانتکست یافت نشود پرتاب می‌شود.</exception>
    TableMetadata GetTableMetadata(string tableName);


    /// <summary>
    ///     دریافت ستون های قابل فیلتر به همراه نوع عملیات
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    public Task<List<TableDisplayMetadata>> GetFilterableColumnsAsync(int reportDefinitionId);

    /// <summary>
    ///     دریافت ستون های قابل مرتب سازی
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    public Task<List<SortableColumn>> GetSortableColumnsAsync(int reportDefinitionId);
}