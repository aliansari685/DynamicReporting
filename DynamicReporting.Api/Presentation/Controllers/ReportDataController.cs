namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-data")]
public class ReportDataController(IReportDataService reportDataService) : ControllerBase
{
    /// <summary>
    /// دریافت دیتا از یک گزارش داینامیک
    /// </summary>
    [HttpGet("{reportDefinitionId:int}")]
    public async Task<IActionResult> GetReportData(int reportDefinitionId)
    {
        var data = await reportDataService.GetReportDataAsync(reportDefinitionId);

        return Ok(data);
    }
}