namespace DynamicReporting.Api.Presentation.Controllers
{
    [Route("api/report-export"), ApiController]
    public class ReportExportController(IReportExportService exportService) : ControllerBase
    {
        const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// ذخیره روی مموری و ساخت سریع برای حجم فایل و تعداد ردیف متوسط 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/saveToRam/{id}")]
        public async Task<IActionResult> ExportWithMemoryStreamAsync(int id, CancellationToken cancellationToken)
        {
            var fileDownloadName = $"report_{Guid.NewGuid()}.xlsx";
            var stream = new MemoryStream();
            await exportService.ExportToExcelAsync(id, stream, cancellationToken);
            stream.Position = 0;
            return File(stream, ContentType, fileDownloadName);
        }

        /// <summary>
        /// ذخیره روی هارد بصورت موقت برای حجم و تعداد ردیف بالا 
        /// </summary>
        /// <param name="id">reportDefinitionId</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("excel/saveToDisk/{id}")]
        public async Task<IActionResult> ExportWithDiskStreamAsync(int id, CancellationToken cancellationToken)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, $"report_{Guid.NewGuid()}.xlsx");

            await using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 2 * 1024 * 1024, useAsync: true))
            {
                await exportService.ExportToExcelAsync(id, fileStream, cancellationToken);
            }
            return PhysicalFile(path, ContentType, Path.GetFileName(path));
            //  return Ok(new { Path = path }); if we need to return path instead of file
        }
    }
}