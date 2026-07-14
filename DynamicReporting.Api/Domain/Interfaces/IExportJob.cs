namespace DynamicReporting.Api.Domain.Interfaces;

public interface IExportJob
{
    /// <summary>
    /// جاب خروجی گرفتن
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش پویا</param>
    /// <param name="filtersList"></param>
    /// <param name="sortColumn">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="type">نوع خروجی مثل اکسل و پی دی اف</param>
    /// <param name="reportGuid">شناسه گزارش ساخته شده</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task ExportJobAsync(int reportDefinitionId, List<FilterCondition>? filtersList, string sortColumn,
        ServiceResolver.ExportType type, Guid reportGuid,
        CancellationToken cancellationToken);

    /// <summary>
    /// تکمیل کردن دیتابیس و مراحل بعد از ساخت گزارش
    /// </summary>
    /// <returns></returns>
    public Task FinalizeExportJobAsync(int jobId, Guid reportGuid);
}