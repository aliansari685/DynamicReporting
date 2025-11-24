namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/[controller]")]
public class ReportDefinitionController : Controller
{
    [HttpGet("[controller]")]
    public IActionResult GetAll()
    {
        return Ok();
    }
    [HttpGet("{id:int}")]
    //  [HttpGet($"{{{nameof(id)}:int}}")]
    public IActionResult GetById([FromQuery] int id)
    {
        return Ok();
    }
}