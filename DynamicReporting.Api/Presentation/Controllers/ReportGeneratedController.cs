namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController]
[Route("api/report-generated")]
public class ReportGeneratedController(IReportGeneratedService generatedService) : ControllerBase
{
    /// <summary>
    ///     دریافت جزییات گزارش
    /// </summary>
    /// <param name="id">شناسه بر اساس جی یو آیدی</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetReportJobDetailsAsync(Guid id)
    {
        var result = await generatedService.GetByGuidAsync(id);
        return Ok(result);
    }

    /// <summary>
    ///     دریافت همه ی لیست
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var result = await generatedService.GetAllToListAsync();
        return Ok(result);
    }

    /// <summary>
    ///     دریافت وضعیت فارسی گزارش و جاب
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("status/{id:guid}")]
    public async Task<ActionResult> GetPersianStatus(Guid id)
    {
        var result = await generatedService.GetStatusPersianByGuid(id);
        return Ok(result);
    }

    /// <summary>
    ///     دانلود فایل گزارش
    /// </summary>
    /// <param name="id">شناسه گزارش</param>
    /// <returns></returns>
    [HttpGet("download/{id:guid}")]
    public async Task<ActionResult> GetDownloadFile(Guid id)
    {
        var status = await generatedService.GetStatusByGuid(id);
        if (string.Equals(status, nameof(HangfireJobState.Succeeded),
                StringComparison.CurrentCultureIgnoreCase))
        {
            var result = await generatedService.GetByGuidAsync(id);
            var downloadUrl = result.DownloadUrl ?? string.Empty;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), downloadUrl);
            var stream = System.IO.File.OpenRead(fullPath);
            Enum.TryParse(result.FileType, true, out ExportType trustType);
            return File(stream, FileTypeNameHelper.GetContentType(result.FileType ?? "excel"), $"Report_{id}{FileTypeNameHelper.GetFileType(trustType)}");
        }

        return NotFound("فایل حذف شده است یا وضعیت آن نامعتبر است");
    }

    /// <summary>
    ///     حذف ردیف گزارش
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveReportAsync(Guid id)
    {
        await generatedService.DeleteAsync(id);
        return NoContent();
    }
}