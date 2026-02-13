namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 5000; // هر batch ۵۰۰۰ ردیف
            var totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            int currentRow = 1;
            bool headerWritten = false;

            for (int offset = 0; offset < totalCount; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // دریافت batch از DataService
                var batch = await reportDataService
                    .GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                if (batch.Count == 0)
                    break;

                // نوشتن header فقط یک بار
                if (!headerWritten)
                {
                    var headers = batch.First().Keys.ToList();
                    for (int col = 0; col < headers.Count; col++)
                    {
                        worksheet.Cells[currentRow, col + 1].Value = headers[col];
                    }
                    headerWritten = true;
                    currentRow++;
                }

                // نوشتن داده‌ها
                foreach (var row in batch)
                {
                    int col = 1;
                    foreach (var value in row.Values)
                    {
                        worksheet.Cells[currentRow, col++].Value = value;
                    }
                    currentRow++;
                }

                // **حالا داده‌های batch را روی stream ذخیره موقت می‌کنیم**
                await package.SaveAsAsync(outputStream, cancellationToken);
            }
        }
    }
}
