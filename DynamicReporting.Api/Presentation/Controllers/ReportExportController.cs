namespace DynamicReporting.Api.Presentation.Controllers
{
    [Route("api/report-export"), ApiController]
    public class ReportExportController(IKeyedServiceProvider serviceProvider, IExportBackgroundJobService exportBackgroundJobService) : ControllerBase
    {
        const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// ذخیره روی مموری و ساخت سریع برای حجم فایل و تعداد ردیف متوسط 
        /// </summary>
        /// <param name="id">شناسه</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/fastExport/{id}")]
        public async Task<IActionResult> ExportWithMemoryStreamAsync(int id, CancellationToken cancellationToken)
        {
            var fileDownloadName = $"report_{Guid.NewGuid()}.xlsx";
            var stream = new MemoryStream();
            var excelService = serviceProvider.GetRequiredKeyedService<IReportExportService>("excel");
            await excelService.ExportAsync(id, stream, cancellationToken);
            stream.Position = 0;
            return File(stream, ContentType, fileDownloadName);
        }

        /// <summary>
        /// ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <returns>jobId</returns>
        [HttpGet("excel/{id}")]
        public IActionResult ExportWithDiskStreamAsync(int id)
        {
            var jobId = exportBackgroundJobService.ExportToExcelInBackground(id);
            return Accepted(jobId, "در حال ساخت گزارش....");
            //todo : هروقت گزارش اماده شد بهش نوتیف میدم
        }

        /// <summary>
        /// ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا با تایپ داینامیک 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="type">نوع خروجی مثل اکسل و پی دی اف</param>
        /// <returns>jobId</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ExportAsync(int id, [FromBody] string type)
        {
            //todo : تکمیلش کن
            var stream = new MemoryStream();

            var exportService = serviceProvider.GetRequiredKeyedService<IReportExportService>(type);

            await exportService.ExportAsync(id, stream);

            stream.Position = 0;
            return File(stream, ContentType, "");

            //  var jobId = exportBackgroundJobService.ExportToExcelInBackgroundAsync(id);
            //   return Accepted(1, "در حال ساخت گزارش....");
        }

    }

}
//تیجه‌گیری برای پروژه شما
// IExportBackgroundJobService (که در کنترلر استفاده می‌شود): در لایه Application تعریف شود.
// IReportExportService (که کار فنی اکسل نویسی را انجام می‌دهد): در لایه Domain تعریف شود و در Infrastructure پیاده‌سازی شود.