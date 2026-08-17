namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportGeneratedService
{
    /// <summary>
    ///     دریافت بر اساس یونیک ایدی
    /// </summary>
    /// <param name="id">جاب ایدی</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ReportGenerationResponseDto> GetByGuidAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     دریافت همه ی ردیف ها به لیست
    /// </summary>
    /// <returns></returns>
    public Task<List<ReportGenerationResponseDto>> GetAllToListAsync();

    /// <summary>
    ///     دریافت وضعیت گزارش
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> GetStatusByGuid(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     دریافت وضعیت فارسی گزارش
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> GetStatusPersianByGuid(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     ایجاد ردیف جدید
    /// </summary>
    /// <param name="dto">مدل ارسالی</param>
    /// <returns></returns>
    public Task<bool> CreateAsync(ReportGenerationRequestDto dto);

    /// <summary>
    ///     حذف ردیف و جاب
    /// </summary>
    /// <param name="id">شناسه ردیف یا همان جاب ایدی</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    public Task<bool> UpdateAsync(ReportGenerationUpdateDto dto, CancellationToken cancellationToken);
}