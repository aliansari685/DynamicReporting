namespace DynamicReporting.Mvc.Controllers;

public sealed class ReportsController(
    IReportDefinitionService definitionService,
    IMetadataService metadataService) : Controller
{
    /// <summary>
    ///     صفحه اصلی گزارش‌ها
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        int? reportId = null,
        CancellationToken cancellationToken = default)
    {
        var reportsTask =
            definitionService.GetReportsAsync(cancellationToken);

        var defaultReportTask =
            definitionService.GetDefaultReportAsync(cancellationToken);

        var metadataTask =
            metadataService.GetAllMetadataAsync(cancellationToken);

        await Task.WhenAll(
            reportsTask,
            defaultReportTask,
            metadataTask);

        var reports = await reportsTask;
        var defaultReport = await defaultReportTask;
        var metadata = await metadataTask;

        var selectedReport = reportId.HasValue
            ? await definitionService.GetReportAsync(
                reportId.Value,
                cancellationToken)
            : defaultReport;

        var tableDisplayNames = metadata
            .ToDictionary(
                x => x.TableName,
                x => x.DisplayName ?? x.TableName,
                StringComparer.OrdinalIgnoreCase);

        ViewBag.Metadata = metadata;
        ViewBag.TableDisplayNames = tableDisplayNames;

        var model = new ReportsIndexVm
        {
            Reports = reports,
            DefaultReport = defaultReport,
            SelectedReport = selectedReport
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest();

        var report = await definitionService.GetReportAsync(
            id,
            cancellationToken);

        if (report is null)
            return NotFound();

        return Json(report);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromBody] ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await definitionService.CreateReportAsync(
            model,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message = "گزارش با موفقیت ایجاد شد."
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] ReportDefinitionEditVm model,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var report = await definitionService.GetReportAsync(
            id,
            cancellationToken);

        if (report is null)
            return NotFound();

        await definitionService.UpdateReportAsync(
            id,
            model,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message = "گزارش با موفقیت ویرایش شد."
        });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest();

        var deleted = await definitionService.DeleteReportAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}