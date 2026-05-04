namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-data")]
public class ReportDataController(IReportDataService reportDataService) : ControllerBase
{
    /// <summary>
    /// دریافت دیتا و ردیف ها از یک گزارش داینامیک
    /// </summary>
    /// <param name="reportDefinitionId">شناسه ردیف</param>
    /// <param name="filters">فیلتر ها</param>
    /// <param name="page">صفحه ی چند</param>
    /// <param name="take">تعداد ردیف ها</param>
    /// <returns>خروجی جیسون لیست</returns>
    [HttpGet("{reportDefinitionId:int}")]
    public async Task<ActionResult<PagedResult<Dictionary<string, object?>>>> GetReportData(int reportDefinitionId,
        [FromQuery] string? filters,
        [FromQuery] int page = 1, [FromQuery] int take = 10)
    {
        if (take is <= 0 or > 1000)
            return BadRequest("تعداد رکورد در هر صفحه معتبر نیست.");

        //todo: اگه فیلتر خالی بود فیلتر پیش فرض اضافه کن
        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");

        var result = await reportDataService.GetReportDataAsync(reportDefinitionId, filtersList, page, take);

        return Ok(result);
    }
}