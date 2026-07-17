namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-data")]
public class ReportDataController(IReportDataService reportDataService, IReportMetadataService metadataService) : ControllerBase
{
    /// <summary>
    /// دریافت دیتا و ردیف ها از یک گزارش داینامیک
    /// </summary>
    /// <param name="reportDefinitionId">شناسه ردیف</param>
    /// <param name="filters">فیلتر ها</param>
    /// <param name="sortColumn"></param>
    /// <param name="page">صفحه ی چند</param>
    /// <param name="take">تعداد ردیف ها</param>
    /// <returns>خروجی جیسون لیست</returns>
    [HttpGet("{reportDefinitionId:int}")]
    public async Task<ActionResult<PagedResult<Dictionary<string, object?>>>> GetReportData(int reportDefinitionId,
        [FromQuery] string? filters, [FromQuery] string? sortColumn,
        [FromQuery] int page = 1, [FromQuery] int take = 10)
    {
        if (take is <= 0 or > 1000)
            return BadRequest("تعداد رکورد در هر صفحه معتبر نیست.");

        if (!string.IsNullOrEmpty(sortColumn))
        {
            if (!sortColumn.Contains('.'))
                return BadRequest("فرمت مرتب‌سازی نامعتبر است. فرمت صحیح: Table.Column");

            // جلوگیری از SQL Injection (بررسی کاراکترهای خطرناک)
            if (sortColumn.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار مرتب‌سازی حاوی کاراکترهای غیرمجاز است.");
        }

        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");

        var result = await reportDataService.GetReportDataAsync(reportDefinitionId, filtersList, sortColumn ?? "", page, take);

        return Ok(result);
    }

    /// <summary>
    /// دریافت ستون های قابل فیلتر گزارش مربوطه
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    [HttpGet("{reportDefinitionId:int}/filterable-columns")]
    public async Task<ActionResult> GetFilterableColumns(int reportDefinitionId)
    {
        var res = await metadataService.GetFilterableColumnsAsync(reportDefinitionId);
        return Ok(res);
    }

    /// <summary>
    /// دریافت ستون های قابل مرتب سازی گزارش مربوطه
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    [HttpGet("{reportDefinitionId:int}/sortable-columns")]
    public async Task<ActionResult> GetSortableColumns(int reportDefinitionId)
    {
        var res = await metadataService.GetSortableColumnsAsync(reportDefinitionId);
        return Ok(res);
    }
}