namespace DynamicReporting.Mvc.Controllers;

public class ReportsController(
    IReportService reportService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> Definitions(
        CancellationToken cancellationToken)
    {
        var reports =
            await reportService.GetReportsAsync(
                cancellationToken);

        return Json(reports);
    }


    [HttpGet]
    public async Task<IActionResult> Default(
        CancellationToken cancellationToken)
    {
        var report =
            await reportService.GetDefaultReportAsync(
                cancellationToken);

        if (report is null)
        {
            return NotFound();
        }

        return Json(report);
    }


    [HttpGet]
    public async Task<IActionResult> Data(
        int id,
        string? filters,
        string? sort,
        string? dir,
        int page = 1,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result =
            await reportService.GetReportDataAsync(
                id,
                filters,
                sort,
                dir,
                page,
                take,
                cancellationToken);

        return Json(result);
    }


    [HttpGet]
    public async Task<IActionResult> FilterableColumns(
        int id,
        CancellationToken cancellationToken)
    {
        var columns =
            await reportService.GetFilterableColumnsAsync(
                id,
                cancellationToken);

        return Json(columns);
    }


    [HttpGet]
    public async Task<IActionResult> SortableColumns(
        int id,
        CancellationToken cancellationToken)
    {
        var columns =
            await reportService.GetSortableColumnsAsync(
                id,
                cancellationToken);

        return Json(columns);
    }
}

