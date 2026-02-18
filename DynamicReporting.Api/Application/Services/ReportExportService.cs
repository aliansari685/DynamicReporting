using System.Diagnostics;
using System.Globalization;

namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            Program.Stopwatch1.Restart();
            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);
            Program.Stopwatch1.Stop();
            Log.Error($"totalCount: + {Program.Stopwatch1.ElapsedMilliseconds}");

            Program.Stopwatch1.Restart();
            int batchSize = Math.Clamp(totalCount / 100, 5000, 15000);
            Program.Stopwatch1.Stop();
            Log.Error($"batchSize: + {Program.Stopwatch1.ElapsedMilliseconds}");

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);


            bool headerWritten = false;
            List<string>? headerKeys = null; // keep header order from the first batch

            //    for (int offset = 0; offset < 100000 /*totalCount*/; offset += batchSize)
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
                    headerWritten = true;
                }
                Program.Stopwatch1.Restart();

                foreach (var rowDict in batch)
                {
                    var rowCells = new List<Cell>(headerKeys!.Count);
                    rowCells.AddRange(headerKeys!.Select(key =>
                    {
                        var has = rowDict.TryGetValue(key, out var val);
                        var cellValue = has ? val ?? string.Empty : string.Empty;
                        return new Cell(cellValue.ToString());
                    }));

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