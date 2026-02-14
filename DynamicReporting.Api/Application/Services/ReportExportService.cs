namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 5000;
            var totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            int currentRow = 1;
            bool headerWritten = false;

            for (int offset = 0; offset < totalCount; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService
                    .GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                if (batch.Count == 0)
                    break;

                worksheet.Cells[currentRow, 1].LoadFromDictionaries(batch, printHeaders: !headerWritten);

                if (!headerWritten)
                {
                    headerWritten = true;
                    currentRow++;
                }

                currentRow += batch.Count;
            }

            await package.SaveAsAsync(outputStream, cancellationToken);
        }

        //public async Task ExportToExcel_ClosedXmlAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        //{
        //    const int batchSize = 5000;

        //    // تعداد کل رکوردها
        //    var totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

        //    // Open a new workbook
        //    using var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("Report");

        //    int currentRow = 1;
        //    bool headerWritten = false;

        //    for (int offset = 0; offset < totalCount; offset += batchSize)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        // گرفتن batch از دیتابیس
        //        var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

        //        if (batch.Count == 0)
        //            break;

        //        // اگر header نوشته نشده، ستون‌ها را اضافه می‌کنیم
        //        if (!headerWritten)
        //        {
        //            var headers = batch.First().Keys.ToList();
        //            for (int col = 0; col < headers.Count; col++)
        //                worksheet.Cell(currentRow, col + 1).Value = headers[col];

        //            headerWritten = true;
        //            currentRow++;
        //        }

        //        // اضافه کردن هر ردیف به Worksheet
        //        foreach (var row in batch)
        //        {
        //            int col = 1;
        //            foreach (var value in row.Values)
        //            {
        //                worksheet.Cell(currentRow, col++).Value = value;
        //            }
        //            currentRow++;
        //        }

        //        // Flush کردن داده‌ها به stream با OpenXmlWriter
        //        using var writer = worksheet.GetOpenXmlWriter();
        //        writer.Flush(); // این مرحله کمک می‌کند سلول‌ها در RAM نگه داشته نشوند
        //    }

        //    // ذخیره نهایی روی Stream خروجی
        //    await workbook.SaveAsAsync(outputStream, cancellationToken);
        //}



    }
}
