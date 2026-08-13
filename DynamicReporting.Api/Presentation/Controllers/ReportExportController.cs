using DynamicReporting.Api.Domain.Enums;

namespace DynamicReporting.Api.Presentation.Controllers;

[Route("api/report-export")]
[ApiController]
public class ReportExportController(IExportBackgroundJobService exportBackgroundJobService) : ControllerBase
{
    /// <summary>
    ///     ذخیره روی مموری و ساخت سریع برای حجم فایل و تعداد ردیف متوسط
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <param name="filters"></param>
    /// <param name="sort">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="dir">صعودی یا نزولی ؟ || asc-desc</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("excel/fastExport/{id}")]
    public async Task<IActionResult> ExportWithMemoryStreamAsync(int id, [FromQuery] string? filters,
        [FromQuery] string? sort, [FromQuery] string? dir, CancellationToken cancellationToken)
    {
        var valueTuple = ValidateRequest(filters, sort, dir);
        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");
        var memoryStream =
            await exportBackgroundJobService.ExportDirectAsync(id, filtersList, valueTuple.dto, cancellationToken);
        var fileDownloadName = $"report_{Guid.NewGuid()}.xlsx";
        return File(memoryStream, FileTypeNameHelper.GetContentType("excel"), fileDownloadName);
    }

    /// <summary>
    ///     ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا با تایپ داینامیک
    /// </summary>
    /// <param name="id">reportDefinitionId</param>
    /// <param name="filters">شرط ها</param>
    /// <param name="sort">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="dir">صعودی یا نزولی ؟ || asc-desc</param>
    /// <param name="type">نوع خروجی مثل pdf , csv, excel</param>
    /// <returns>jobId</returns>
    [HttpGet("export/{id}")]
    public async Task<IActionResult> ExportAsync(int id, [FromQuery] string? filters, [FromQuery] string? sort,
        [FromQuery] string? dir, [FromQuery] string type = "excel")
    {
        var valueTuple = ValidateRequest(filters, sort, dir, type);

        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");

        var jobId = await exportBackgroundJobService.ExportInBackgroundAsync(id, filtersList, valueTuple.dto,
            valueTuple.exportType);

        return Accepted($"api/report-generated/status/{jobId}",
            new
            {
                reportid = jobId.ToString(),
                message = "در حال ساخت گزارش ، به محض اماده شدن گزارش اطلاع میدم"
            });
    }

    #region Helper Method

    /// <summary>
    ///     ولیدیشن مقادیر ورودی کنترلر
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="sort"></param>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private static (SortableColumnDto dto, ExportType exportType) ValidateRequest(string? filters,
        string? sort, string? dir, string type = "excel")
    {
        (SortableColumnDto dto, ExportType exportType) tuple = new();

        if (!Enum.TryParse(type, true, out tuple.exportType))
            throw new InvalidOperationException(
                $"مقدار '{type}' معتبر نیست. مقادیر مجاز: {string.Join(", ", Enum.GetNames<ExportType>())}");

        if (ContainsDangerousCharacters(filters))
            throw new ArgumentException(
                "مقدار فیلترهای ارسالی حاوی کاراکترهای غیرمجاز است.",
                nameof(filters));

        if (ContainsDangerousCharacters(dir))
            throw new ArgumentException(
                "مقدار مرتب‌سازی صعودی یا نزولی حاوی کاراکترهای غیرمجاز است.",
                nameof(dir));

        var sortableColumn = new SortableColumnDto();

        if (string.IsNullOrWhiteSpace(sort))
        {
            tuple.dto = sortableColumn;
            return tuple;
        }

        if (ContainsDangerousCharacters(sort))
            throw new ArgumentException(
                "مقدار مرتب‌سازی حاوی کاراکترهای غیرمجاز است.",
                nameof(sort));

        if (!sort.Contains('.'))
            throw new ArgumentException(
                "فرمت مرتب‌سازی نامعتبر است. فرمت صحیح: Table.Column",
                nameof(sort));

        sortableColumn.Column = sort;

        sortableColumn.SortDirection = Enum.TryParse<SortDirection>(dir, true, out var sortDirection)
            ? sortDirection
            : throw new ArgumentException(
                $"مقدار '{dir}' برای مرتب‌سازی معتبر نیست.",
                nameof(dir));

        return tuple;
    }

    /// <summary>
    ///     جلوگیری از SQL Injection(بررسی کاراکترهای خطرناک)
    /// </summary>
    /// <param name="value">مقدار صحت سنجی</param>
    /// <returns></returns>
    private static bool ContainsDangerousCharacters(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
               && value.Any(c => "';--/*".Contains(c));
    }

    #endregion
}