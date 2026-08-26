namespace DynamicReporting.Mvc.Controllers;

public sealed class MyReportsController(
    IReportDefinitionService definitionService,
    IMetadataService metadataService) : Controller
{
    /// <summary>
    /// صفحه گزارش‌های من
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken = default)
    {
        var reportsTask =
            definitionService.GetReportsAsync(
                cancellationToken);

        var metadataTask =
            metadataService.GetAllMetadataAsync(
                cancellationToken);

        await Task.WhenAll(
            reportsTask,
            metadataTask);

        var reports =
            await reportsTask;

        var metadata =
            await metadataTask;

        var tableDisplayNames =
            metadata.ToDictionary(
                x => x.TableName,
                x => x.DisplayName ?? x.TableName,
                StringComparer.OrdinalIgnoreCase);

        ViewBag.TableDisplayNames =
            tableDisplayNames;
        ViewBag.Metadata = metadata;

        return View(reports);
    }
}