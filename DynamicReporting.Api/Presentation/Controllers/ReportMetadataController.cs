namespace DynamicReporting.Api.Presentation.Controllers;

/// <summary>
///     وب سرویس برای دریافت اطلاعات متادیتای جداول دیتابیس.
///     شامل دریافت تمامی جداول، ستون‌ها و متادیتای یک جدول خاص.
/// </summary>
[ApiController]
[Route("api/report-metadata")]
public class ReportMetadataController(IReportMetadataService metadataService) : ControllerBase
{
    /// <summary>
    ///     دریافت نام تمام جدول‌های دیتابیس.
    /// </summary>
    /// <returns>لیست نام جدول‌ها</returns>
    [HttpGet("tables")]
    public IActionResult GetAllTables()
    {
        var result = metadataService.GetAllTableNames();
        return Ok(result);
    }

    /// <summary>
    ///     دریافت متادیتای تمامی جدول‌های دیتابیس شامل نام جدول و ستون‌ها با عناوین فارسی.
    /// </summary>
    /// <returns>لیست کامل متادیتای جدول‌ها</returns>
    [HttpGet("metadata")]
    public IActionResult GetAllMetadata()
    {
        var result = metadataService.GetAllMetadata();
        return Ok(result);
    }

    /// <summary>
    ///     دریافت متادیتای یک جدول مشخص شامل نام جدول و ستون‌های آن
    /// </summary>
    /// <param name="tableName">نام جدول موردنظر Customers</param>
    /// <returns>متادیتای جدول موردنظر</returns>
    [HttpGet("metadata/{tableName}")]
    public IActionResult GetTableMetadata(string tableName)
    {
        var result = metadataService.GetTableMetadata(tableName);
        return Ok(result);
    }
}