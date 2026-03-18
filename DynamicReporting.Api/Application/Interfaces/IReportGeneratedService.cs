namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportGeneratedService
{
    /// <summary>
    /// دریافت بر اساس یونیک ایدی
    /// </summary>
    /// <param name="id">جاب ایدی</param>
    /// <returns></returns>
    public Task<ReportGeneration> GetByGuidAsync(Guid id);

    /// <summary>
    /// دریافت همه ی ردیف ها
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ReportGeneration> GetAll();
    /// <summary>
    /// دریافت همه ی ردیف ها به لیست
    /// </summary>
    /// <returns></returns>
    public Task<List<ReportGeneration>> GetAllToListAsync();
    /// <summary>
    /// ایجاد ردیف جدید
    /// </summary>
    /// <param name="dto">مدل ارسالی</param>
    /// <returns></returns>
    public Task CreateAsync(ReportGenerationDto dto);
    /// <summary>
    /// حذف بر اساس یونیک ایدی
    /// </summary>
    /// <param name="id">شناسه ردیف یا همان جاب ایدی</param>
    /// <returns></returns>
    public Task DeleteAsync(Guid id);
}