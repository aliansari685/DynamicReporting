namespace DynamicReporting.Mvc.Controllers;

public sealed class ReportExportController(IReportExportService reportService) : Controller
{
    /// <summary>
    /// خروجی سریع Excel
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> FastExport(
        int reportDefinitionId,
        string? filters = null,
        string? sort = null,
        string? dir = null,
        CancellationToken cancellationToken = default)
    {
        if (reportDefinitionId <= 0)
            return BadRequest();

        var response =
            await reportService.FastExportAsync(
                reportDefinitionId,
                filters,
                sort,
                dir,
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
            ?? $"report_{Guid.NewGuid()}.xlsx";

        return File(
            content,
            contentType,
            fileName.Trim('"'));
    }

    /// <summary>
    /// شروع عملیات Export در Background
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Export(
        int reportDefinitionId,
        string? filters = null,
        string? sort = null,
        string? dir = null,
        string type = "excel",
        CancellationToken cancellationToken = default)
    {
        if (reportDefinitionId <= 0)
            return BadRequest();

        var result =
            await reportService.ExportAsync(
                reportDefinitionId,
                filters,
                sort,
                dir,
                type,
                cancellationToken);

        return Json(result);
    }
}