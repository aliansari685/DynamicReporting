namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-data")]
public class ReportDataController(IReportDataService reportDataService) : ControllerBase
{

    /// <summary>
    /// دریافت دیتا و ردیف ها از یک گزارش داینامیک
    /// </summary>
    /// <param name="reportDefinitionId">شناسه ردیف</param>
    /// <returns></returns>
    [HttpGet("{reportDefinitionId:int}")]
    public async Task<IActionResult> GetReportData(int reportDefinitionId)
    {
        var data = await reportDataService.GetReportDataAsync(reportDefinitionId);

        return Ok(data);
    }
}