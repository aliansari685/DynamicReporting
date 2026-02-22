namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            int batchSize = Math.Clamp(totalCount / 100, 5000, 15000);
            //  int batchSize = 5000;

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);


            bool headerWritten = false;
            List<string>? headerKeys = null; // keep header order from the first batch

            //     for (int offset = 0; offset < 100000 /*totalCount*/; offset += batchSize) convert to while and remove get count
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, 0 /*offset*/, batchSize);

                if (batch.Count == 0)
                    throw new NullReferenceException("");
                //break;

                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();
                    var headerCells = headerKeys.Select(k => new Cell(k)).ToList();
                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);
                }
                Program.Stopwatch1.Restart();

                foreach (var rowDict in batch)
                {
                    var rowCells = new List<Cell>(headerKeys!.Count);
                    foreach (var key in headerKeys)
                    {
                        rowDict.TryGetValue(key, out var val);
                        string cellText = val?.ToString() ?? string.Empty;
                        rowCells.Add(new Cell(cellText));
                    }

                    await spreadsheet.AddRowAsync(rowCells, cancellationToken);
                }

                Program.Stopwatch1.Stop();
                Log.Error($"After Fill Excel: + {Program.Stopwatch1.ElapsedMilliseconds}");
            }

            // Finalize the XLSX file (important to call before disposing).
            await spreadsheet.FinishAsync(cancellationToken);
        }

    }
}