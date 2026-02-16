namespace DynamicReporting.Api.Presentation.Controllers
{
    [Route("api/report-export"), ApiController]
    public class ReportExportController(IReportExportService exportService) : ControllerBase
    {
        /// <summary>
        /// ذخیره روی مموری و ساخت سریع برای حجم فایل و تعداد ردیف متوسط 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/saveToRam/{id}")]
        public async Task<IActionResult> ExportWithMemoryStreamAsync(int id, CancellationToken cancellationToken)
        {
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileDownloadName = $"report_{id}.xlsx";
            var stream = new MemoryStream();
            await exportService.ExportToExcelAsync(id, stream, cancellationToken);
            stream.Position = 0;
            return File(stream, contentType, fileDownloadName);
        }

        /// <summary>
        /// ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/saveToDisk/{id}")]
        public async Task<IActionResult> ExportWithDiskStreamAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, $"report_{Guid.NewGuid()}.xlsx");

            await using (var fileStream = new FileStream(
                             path,
                             FileMode.Create,
                             FileAccess.Write,
                             FileShare.None,
                             bufferSize: 81920,
                             useAsync: true))
            {
                await exportService.ExportToExcelAsync(id, fileStream, cancellationToken);
            }

            return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));

            //  return Ok(new { Path = path });
        }


        /// <summary>
        /// ساخت و دانلود مستقیم در لحظه روی سیستم کلاینت
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/saveToNetwork/{id}")]
        public async Task<IActionResult> ExportWithNetworkStreamAsync(int id, CancellationToken cancellationToken)
        {
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=report_{id}.xlsx");
            Stream stream = Response.Body;
            await exportService.ExportToExcelFastAsync(id, stream, cancellationToken);
            return new EmptyResult();
        }
    }
}