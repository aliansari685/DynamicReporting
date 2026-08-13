namespace DynamicReporting.Mvc.Controllers;

public sealed class ReportDataController(
    IReportDataService reportService) : Controller
{
    /// <summary>
    /// دریافت اطلاعات گزارش به صورت صفحه‌بندی شده
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Data(
        int reportDefinitionId,
        string? filters = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        if (reportDefinitionId <= 0)
            return BadRequest();

        if (page < 1)
            page = 1;

        if (take < 1)
            take = 10;

        var result = await reportService.GetReportDataAsync(
            reportDefinitionId,
            filters,
            sort,
            dir,
            page,
            take,
            cancellationToken);

        return Json(result);
    }

    /// <summary>
    /// دریافت ستون‌های قابل فیلتر
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> FilterableColumns(
        int reportDefinitionId,
        CancellationToken cancellationToken = default)
    {
        if (reportDefinitionId <= 0)
            return BadRequest();

        var result =
            await reportService.GetFilterableColumnsAsync(
                reportDefinitionId,
                cancellationToken);

        return Json(result);
    }

    /// <summary>
    /// دریافت ستون‌های قابل مرتب‌سازی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SortableColumns(
        int reportDefinitionId,
        CancellationToken cancellationToken = default)
    {
        if (reportDefinitionId <= 0)
            return BadRequest();

        var result =
            await reportService.GetSortableColumnsAsync(
                reportDefinitionId,
                cancellationToken);

        return Json(result);
    }
}