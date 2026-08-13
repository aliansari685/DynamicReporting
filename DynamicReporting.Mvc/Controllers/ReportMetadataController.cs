namespace DynamicReporting.Mvc.Controllers;

public sealed class ReportMetadataController(
    IMetadataService metadataService) : Controller
{
    /// <summary>
    /// نمایش تمام جداول دیتابیس
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken = default)
    {
        var tables = await metadataService.GetAllTablesAsync(
            cancellationToken);

        return View(tables);
    }

    /// <summary>
    /// نمایش متادیتای تمام جداول
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> AllMetadata(
        CancellationToken cancellationToken = default)
    {
        var metadata = await metadataService.GetAllMetadataAsync(
            cancellationToken);

        return View(metadata);
    }

    /// <summary>
    /// نمایش متادیتای یک جدول مشخص
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return RedirectToAction(nameof(Index));

        var metadata = await metadataService.GetTableMetadataAsync(
            tableName);

        if (metadata is not null) return View(metadata);
        TempData["ErrorMessage"] =
            $"جدول '{tableName}' یافت نشد.";

        return RedirectToAction(nameof(Index));

    }
}