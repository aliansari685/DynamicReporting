namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-generated")]
public class ReportGeneratedController(IReportGeneratedService generatedService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetReportJobDetailsAsync(Guid id)
    {
        var result = await generatedService.GetByGuidAsync(id);
        return Ok(result);
    }
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        var result = await generatedService.GetAllToListAsync();
        return Ok(result);
    }

    [HttpGet("getStatus/{id:guid}")]
    public async Task<ActionResult> GetPersianStatus(Guid id)
    {
        var result = await generatedService.GetStatusByGuid(id);
        return Ok(result);
    }

    [HttpGet("download/{id:guid}")]
    public async Task<ActionResult> GetDownloadFile(Guid id)
    {
        var result = await generatedService.GetByGuidAsync(id);
        var downloadUrl = result.DownloadUrl ?? string.Empty;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), downloadUrl);
        var stream = System.IO.File.OpenRead(fullPath);
        return File(stream, FileTypeNameHelper.GetContentType(result.FileType ?? "excel"), $"Report_{id}");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveReportAsync(Guid id)
    {
        await generatedService.DeleteAsync(id);
        return NoContent();
    }
}