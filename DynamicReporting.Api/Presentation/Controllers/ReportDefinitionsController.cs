namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-definitions")]
public class ReportDefinitionsController(IReportDefinitionService reportDefinitionService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await reportDefinitionService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await reportDefinitionService.GetByIdAsync(id));
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefault()
    {
        return Ok(await reportDefinitionService.GetByPropertyAsync(x => x.IsDefault == true));
    }

    //   [HttpPost]
    //   // public async Task<IActionResult> Create([FromBody] CreateReportDefinitionDto dto) { }
    //
    //   [HttpPut("{id:int}")]
    //   //  public async Task<IActionResult> Update(int id, [FromBody] UpdateReportDefinitionDto dto) { }
    //
    //   [HttpDelete("{id:int}")]
    //  public async Task<IActionResult> Delete(int id) { }
}