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
    public ActionResult GetPersianStatus(Guid id)
    {
        var result = generatedService.GetStatus(id);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveReportAsync(Guid id)
    {
        await generatedService.DeleteAsync(id);
        return NoContent();
    }
}