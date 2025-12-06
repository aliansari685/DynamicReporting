namespace DynamicReporting.Api.Presentation.Controllers;

[ApiController, Route("api/report-definitions")]
public class ReportDefinitionsController(IReportDefinitionService reportDefinitionService) : ControllerBase
{
    /// <summary>
    /// دریافت تمام قالب‌های گزارش
    /// </summary>
    /// <returns>لیست تمام قالب‌ها</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await reportDefinitionService.GetAllAsync());
    }

    /// <summary>
    /// دریافت قالب گزارش بر اساس شناسه
    /// </summary>
    /// <param name="id">شناسه قالب گزارش</param>
    /// <returns>قالب گزارش مورد نظر</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await reportDefinitionService.GetByIdAsync(id));
    }

    /// <summary>
    /// دریافت قالب گزارش پیش‌ فرض
    /// </summary>
    /// <returns>قالب پیش ‌فرض</returns>
    [HttpGet("default")]
    public async Task<IActionResult> GetDefault()
    {
        return Ok(await reportDefinitionService.GetByPropertyAsync(x => x.IsDefault == true));
    }

    /// <summary>
    /// ایجاد قالب گزارش جدید
    /// </summary>
    /// <param name="dto">اطلاعات قالب گزارش</param>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReportDefinitionDto dto)
    {
        await reportDefinitionService.CreateAsync(dto);
        return Created();
    }

    /// <summary>
    /// به‌روزرسانی یک قالب گزارش موجود
    /// </summary>
    /// <param name="id">شناسه قالب گزارش</param>
    /// <param name="dto">اطلاعات به‌روزرسانی شده</param>
    /// <returns>HTTP 204 NoContent در صورت موفقیت</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportDefinitionDto dto)
    {
        await reportDefinitionService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// حذف یک قالب گزارش
    /// </summary>
    /// <param name="id">شناسه قالب گزارش</param>
    /// <returns>HTTP 204 NoContent در صورت موفقیت</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await reportDefinitionService.DeleteAsync(id);
        return NoContent();
    }
}