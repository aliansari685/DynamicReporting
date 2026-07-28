namespace DynamicReporting.Api.Presentation.Controllers;

[Route("api/report-export")]
[ApiController]
public class ReportExportController(
    IServiceResolver serviceProvider,
    IExportBackgroundJobService exportBackgroundJobService) : ControllerBase
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
        if (!string.IsNullOrEmpty(filters))
            // جلوگیری از SQL Injection (بررسی کاراکترهای خطرناک)
            if (filters.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار فیلتر های ارسالی حاوی کاراکترهای غیرمجاز است.");
        if (!string.IsNullOrEmpty(dir))
            if (dir.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار مرتب‌سازی صعودی یا نزولی حاوی کاراکترهای غیرمجاز است.");

        SortableColumnDto sortableColumnDto = new();

        if (!string.IsNullOrEmpty(sort))
        {
            if (sort.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار مرتب‌سازی حاوی کاراکترهای غیرمجاز است.");

            if (!sort.Contains('.'))
                return BadRequest("فرمت مرتب‌سازی نامعتبر است. فرمت صحیح: Table.Column");

            sortableColumnDto.Column = sort;

            Enum.TryParse<SortDirection>(dir, true, out var sortDirection);

            sortableColumnDto.SortDirection = sortDirection;
        }

        var excelService = serviceProvider.GetExportService(ServiceResolver.ExportType.Excel);
        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");

        var stream = new MemoryStream();

        await excelService.ExportWithAutoFitColumnsAsync(id, filtersList, stream, sortableColumnDto, cancellationToken);

        stream.Position = 0;
        var fileDownloadName = $"report_{Guid.NewGuid()}.xlsx";
        return File(stream, FileTypeNameHelper.GetContentType("excel"), fileDownloadName);
    }

    /// <summary>
    ///     ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا با تایپ داینامیک
    /// </summary>
    /// <param name="id">reportDefinitionId</param>
    /// <param name="filters">شرط ها</param>
    /// <param name="sort">مرتب سازی بر اساس کدام ستون</param>
    /// <param name="dir">صعودی یا نزولی ؟ || asc-desc</param>
    /// <param name="type">نوع خروجی مثل pdf, excel</param>
    /// <returns>jobId</returns>
    [HttpGet("export/{id}")]
    public async Task<IActionResult> ExportAsync(int id, [FromQuery] string? filters, [FromQuery] string? sort,
        [FromQuery] string? dir, [FromQuery] string type = "excel")
    {
        if (!string.IsNullOrEmpty(filters))
            // جلوگیری از SQL Injection (بررسی کاراکترهای خطرناک)
            if (filters.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار فیلتر های ارسالی حاوی کاراکترهای غیرمجاز است.");
        if (!string.IsNullOrEmpty(dir))
            if (dir.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار مرتب‌سازی صعودی یا نزولی حاوی کاراکترهای غیرمجاز است.");

        SortableColumnDto sortableColumnDto = new();

        if (!string.IsNullOrEmpty(sort))
        {
            if (sort.Any(c => "';--/*".Contains(c)))
                return BadRequest("مقدار مرتب‌سازی حاوی کاراکترهای غیرمجاز است.");

            if (!sort.Contains('.'))
                return BadRequest("فرمت مرتب‌سازی نامعتبر است. فرمت صحیح: Table.Column");

            sortableColumnDto.Column = sort;

            Enum.TryParse<SortDirection>(dir, true, out var sortDirection);

            sortableColumnDto.SortDirection = sortDirection;
        }

        var filtersList = JsonConvert.DeserializeObject<List<FilterCondition>>(filters ?? "");

        if (!Enum.TryParse<ServiceResolver.ExportType>(type, true, out var exportType))
        {
            throw new InvalidOperationException($"مقدار '{type}' معتبر نیست. مقادیر مجاز: {string.Join(", ", Enum.GetNames<ServiceResolver.ExportType>())}");
        }
        serviceProvider.GetExportService(exportType);

        var jobId = await exportBackgroundJobService.ExportInBackground(id, filtersList, sortableColumnDto, exportType);

        return Accepted($"api/report-generated/status/{jobId}",
            new
            {
                reportid = jobId.ToString(),
                message = "در حال ساخت گزارش ، به محض اماده شدن گزارش اطلاع میدم"
            });
    }
}