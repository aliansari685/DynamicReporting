namespace DynamicReporting.Mvc.Controllers;

public class ReportGeneratedController(IReportGeneratedService reportService) : Controller
{
    [HttpGet] 
    public IActionResult Index()
    { 
        return View(); 
    }

    /// <summary>
    ///     دریافت تمام گزارش‌های تولید شده
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GeneratedReports(
        CancellationToken cancellationToken = default)
    {
        var reports =
            await reportService.GetGeneratedReportsAsync(
                cancellationToken);

        return Json(reports);
    }


    /// <summary>
    ///     دریافت اطلاعات یک گزارش تولید شده
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GeneratedReport(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var report =
            await reportService.GetGeneratedReportAsync(
                id,
                cancellationToken);

        return Json(report);
    }


    /// <summary>
    ///     دریافت وضعیت گزارش تولید شده
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GeneratedReportStatus(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var status =
            await reportService.GetGeneratedReportStatusAsync(
                id,
                cancellationToken);

        return Content(status);
    }


    /// <summary>
    ///     دانلود گزارش تولید شده
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DownloadGeneratedReport(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var response =
            await reportService.DownloadGeneratedReportAsync(
                id,
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            return StatusCode(
                (int)response.StatusCode,
                error);
        }

        var content =
            await response.Content.ReadAsByteArrayAsync(
                cancellationToken);

        var contentType =
            response.Content.Headers.ContentType?.ToString()
            ?? "application/octet-stream";

        var fileName =
            response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"report_{id}";

        return File(
            content,
            contentType,
            fileName.Trim('"'));
    }


    /// <summary>
    ///     حذف گزارش تولید شده
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteGeneratedReport(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var deleted =
            await reportService.DeleteGeneratedReportAsync(
                id,
                cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}