namespace DynamicReporting.Api.Domain.Interfaces;

public interface IReportDataService
{
    /// <summary>
    /// دریافت دیتای ردیف انتخاب شده
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <returns></returns>
    Task<List<Dictionary<string, object>>> GetReportDataAsync(int reportDefinitionId);
}