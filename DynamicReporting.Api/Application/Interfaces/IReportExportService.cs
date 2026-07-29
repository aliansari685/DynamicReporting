namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportExportService
{
    /// <summary>
    ///     گرفتن دیتا مستقیم از دیتابیس بصورت بچ(تقسیم تعداد)
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <param name="filtersList">لیست فیلتر های اعمال شده</param>
    /// <param name="offset">تقسیم بر چند بشه؟</param>
    /// <param name="take">بخش و صفحه ی چندم دیتا؟</param>
    /// <param name="sortColumn">براساس کدام ستون مرتب سازی شود؟</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, int offset, int take, SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// تنظیم خودکار عرض ستون ها با محل ومکان فایل 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Task SetAutoFitColumnsWithPathAsync(string path);

    /// <summary>
    /// تنظیم خودکار عرض ستون ها با استریم
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MemoryStream> SetAutoFitColumnsWithStreamAsync(MemoryStream stream,
        CancellationToken cancellationToken);
}