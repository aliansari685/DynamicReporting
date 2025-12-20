namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportDataService
{
    /// <summary>
    /// دریافت دیتای ردیف انتخاب شده
    /// </summary>
    /// <param name="reportDefinitionId">شناسه قالب</param>
    /// <param name="page">صفحه مورد نظر</param>
    /// <param name="take">تعداد ردیف هر صفحه پیش فرض 10</param>
    /// <returns></returns>

    Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId, int page = 1, int take = 10);
}