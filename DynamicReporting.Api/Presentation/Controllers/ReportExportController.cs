namespace DynamicReporting.Api.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportExportController(IReportExportService exportService) : ControllerBase
    {
        // 1️⃣ Export با MemoryStream (مثلا برای تست / دانلود سریع)
        [HttpGet("{id}/export/mem")]
        public async Task<IActionResult> ExportMemory(int id)
        {
            using var stream = new MemoryStream();
            await exportService.ExportToExcelAsync(id, stream);

            stream.Position = 0; // حتما قبل از خواندن
            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"report_{id}.xlsx");
        }

        // 2️⃣ Export مستقیم روی FileStream (مثلا برای ذخیره روی هارد)
        [HttpGet("{id}/export/file")]
        public async Task<IActionResult> ExportFile(int id)
        {
            var path = Path.Combine("Exports", $"report_{id}.xlsx");
            Directory.CreateDirectory("Exports"); // مطمئن شو پوشه هست

            await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await exportService.ExportToExcelAsync(id, fileStream);

            return Ok(new { Path = path });
        }

        [HttpGet("{id:int}/export")]
        public async Task<IActionResult> Export(int id, CancellationToken cancellationToken)
        {
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Append("Content-Disposition", $"attachment; filename=report_{id}.xlsx");

            await exportService.ExportToExcelAsync(id, Response.Body, cancellationToken);
            return new EmptyResult();
        }
    }
}
