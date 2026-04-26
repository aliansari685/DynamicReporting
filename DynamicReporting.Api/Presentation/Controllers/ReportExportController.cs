namespace DynamicReporting.Api.Presentation.Controllers
{
    [Route("api/report-export"), ApiController]
    public class ReportExportController(IReportExportServiceResolver serviceProvider, IExportBackgroundJobService exportBackgroundJobService) : ControllerBase
    {
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
            var excelService = serviceProvider.GetService("excel");
            await excelService.ExportAsync(id, stream, cancellationToken);
            stream.Position = 0;
            return File(stream, FileTypeNameHelper.GetContentType("excel"), fileDownloadName);
        }

        /// <summary>
        /// ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا با تایپ داینامیک 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="type">نوع خروجی مثل pdf, excel</param>
        /// <returns>jobId</returns>
        [HttpGet("export/{id}")]
        public async Task<IActionResult> ExportAsync(int id, string type)
        {
            var jobId = await exportBackgroundJobService.ExportInBackground(id, type);
            return Accepted($"api/report-generated/status/{jobId}",
                new
                {
                    reportid = jobId.ToString(),
                    message = "در حال ساخت گزارش ، به محض اماده شدن گزارش اطلاع میدم"
                });
        }
    }
}