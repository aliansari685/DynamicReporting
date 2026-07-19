namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportDataService
{
    /// <summary>
    /// دریافت دیتای ردیف انتخاب شده
    /// </summary>
    /// <param name="reportDefinitionId">شناسه قالب</param>
    /// <param name="filtersList"></param>
    /// <param name="sortColumn">مرتب </param>
    /// <param name="page">صفحه مورد نظر</param>
    /// <param name="take">تعداد ردیف هر صفحه پیش فرض 10</param>
    /// <returns></returns>
    Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, SortableColumnDto sortColumn, int page = 1, int take = 10);

    /// <summary>
    /// تعداد کل رکوردها برای export
    /// </summary>
    Task<int> GetTotalCountAsync(int reportDefinitionId,
        (string whereClause, Dictionary<string, object> parameters) definition);
}