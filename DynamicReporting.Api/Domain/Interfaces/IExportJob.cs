namespace DynamicReporting.Api.Domain.Interfaces;

public interface IExportJob
{
    /// <summary>
    ///     جاب خروجی گرفتن
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش پویا</param>
    /// <param name="filtersList"></param>
    /// <param name="sortColumn">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="type">نوع خروجی مثل اکسل و پی دی اف</param>
    /// <param name="reportGuid">شناسه گزارش ساخته شده</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExportJobAsync(int reportDefinitionId, List<FilterCondition>? filtersList, SortableColumnDto sortColumn,
        ExportType type, Guid reportGuid, CancellationToken cancellationToken);

    /// <summary>
    ///     تکمیل کردن دیتابیس و مراحل بعد از ساخت گزارش
    /// </summary>
    /// <returns></returns>
    public Task FinalizeExportJobAsync(Guid reportGuid);


    /// <summary>
    ///     ذخیره روی مموری و ساخت سریع برای حجم فایل و تعداد ردیف متوسط
    /// </summary>
    /// <param name="reportDefinitionId">شناسه</param>
    /// <param name="filtersList"></param>
    /// <param name="sortColumn">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MemoryStream> ExportDirectAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn, CancellationToken cancellationToken);
}