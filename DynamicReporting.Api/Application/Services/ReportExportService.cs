namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelFastAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 1000;
            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            int currentRow = 1;
            bool headerWritten = false;

            for (int offset = 0; offset < 5000 /*totalCount*/; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                List<Dictionary<string, object?>> batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

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

        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 1000;
            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            // Create a streaming spreadsheet that writes directly to the provided stream.
            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            // Start the worksheet (a spreadsheet must have at least one worksheet).
            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null; // keep header order from the first batch

            for (int offset = 0; offset < 5000 /*totalCount*/; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                if (batch.Count == 0)
                    break;

                // determine headers from the first row of the first non-empty batch
                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();
                    var headerCells = headerKeys.Select(k => new Cell(k)).ToList();
                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);
                    headerWritten = true;
                }

                // write rows in the same column order as headerKeys
                foreach (var rowDict in batch)
                {
                    // For each header key, try to get the value (keeps column order stable)
                    var rowCells = headerKeys!
                        .Select(key =>
                        {
                            var has = rowDict.TryGetValue(key, out var val);
                            var cellValue = has ? (val ?? string.Empty) : string.Empty;
                            return new Cell(cellValue as string);
                        }).ToList();

                    await spreadsheet.AddRowAsync(rowCells, cancellationToken);
                }
            }

            // Finalize the XLSX file (important to call before disposing).
            await spreadsheet.FinishAsync(cancellationToken);
        }
    }
}