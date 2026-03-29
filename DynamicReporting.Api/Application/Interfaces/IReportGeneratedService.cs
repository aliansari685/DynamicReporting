namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportGeneratedService
{
    /// <summary>
    /// دریافت بر اساس یونیک ایدی
    /// </summary>
    /// <param name="id">جاب ایدی</param>
    /// <returns></returns>
    public Task<ReportGenerationResponseDto> GetByGuidAsync(Guid id);

    /// <summary>
    /// دریافت همه ی ردیف ها به لیست
    /// </summary>
    /// <returns></returns>
    public Task<List<ReportGenerationResponseDto>> GetAllToListAsync();

    /// <summary>
    /// دریافت وضعیت گزارش
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <returns></returns>
    public Task<string> GetStatusByGuid(Guid id);

    /// <summary>
    /// ایجاد ردیف جدید
    /// </summary>
    /// <param name="dto">مدل ارسالی</param>
    /// <returns></returns>
    public Task<bool> CreateAsync(ReportGenerationRequestDto dto);

    /// <summary>
    /// حذف ردیف و جاب
    /// </summary>
    /// <param name="id">شناسه ردیف یا همان جاب ایدی</param>
    /// <returns></returns>
    public Task DeleteAsync(Guid id);
}