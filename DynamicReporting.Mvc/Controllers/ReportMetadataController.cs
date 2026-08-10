namespace DynamicReporting.Mvc.Controllers;

public class ReportMetadataController(IDynamicReportingApiService apiService) : Controller
{
    /// <summary>
    ///     Display all database tables
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var tables = await apiService.GetAllTablesAsync();
            return View(tables);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading tables");
            TempData["ErrorMessage"] = "خطا در بارگذاری جداول. لطفاً دوباره تلاش کنید.";
            return View(new List<DisplayTableDto>());
        }
    }

    /// <summary>
    ///     Display metadata for all tables with their columns
    /// </summary>
    public async Task<IActionResult> AllMetadata()
    {
        try
        {
            var metadata = await apiService.GetAllMetadataAsync();
            return View(metadata);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading metadata");
            TempData["ErrorMessage"] = "خطا در بارگذاری متادیتا. لطفاً دوباره تلاش کنید.";
            return View(new List<TableMetadataDto>());
        }
    }

    /// <summary>
    ///     Display detailed metadata for a specific table
    /// </summary>
    public async Task<IActionResult> Details(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) return RedirectToAction(nameof(Index));

        try
        {
            var metadata = await apiService.GetTableMetadataAsync(tableName);
            if (metadata == null)
            {
                TempData["ErrorMessage"] = $"جدول '{tableName}' یافت نشد.";
                return RedirectToAction(nameof(Index));
            }

            return View(metadata);

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading table details for {TableName}", tableName);
            TempData["ErrorMessage"] = $"خطا در بارگذاری جزییات جدول '{tableName}'.";
            return RedirectToAction(nameof(Index));
        }
    }
}