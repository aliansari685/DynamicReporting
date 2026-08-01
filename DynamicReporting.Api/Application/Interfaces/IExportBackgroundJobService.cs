namespace DynamicReporting.Api.Application.Interfaces;

public interface IExportBackgroundJobService
{
    /// <summary>
    ///     ساخت خروجی از گزارش در بکگراند
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <param name="filtersList"></param>
    /// <param name="sortColumn">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Guid> ExportInBackgroundAsync(int reportDefinitionId, List<FilterCondition>? filtersList,
        SortableColumnDto sortColumn,
        ServiceResolver.ExportType type,
        CancellationToken cancellationToken = default);

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