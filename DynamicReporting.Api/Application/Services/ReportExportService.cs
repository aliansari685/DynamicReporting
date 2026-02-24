namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcel1Async(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            Program.Stopwatch1.Restart();

            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            int batchSize = Math.Clamp(totalCount / 100, 5000, 15000);
            //  int batchSize = 6000;

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null;

            for (int offset = 0; offset < totalCount; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                Program.Stopwatch1.Stop();
                Log.Error($"Get Data And Count: + {Program.Stopwatch1.ElapsedMilliseconds}");

                if (batch.Count == 0)
                    break;

                //Header:
                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();
                    var headerCells = headerKeys.Select(k => new Cell(k)).ToList();
                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);
                }

                //Data:
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
            }
            await spreadsheet.FinishAsync(cancellationToken);
        }


        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 6000;

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);
            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null;
            Func<object?, Cell>[]? converters = null;
            Cell[]? rowCells = null;

            int offset = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                if (batch.Count == 0)
                    break;

                // Header + TypeMap فقط بار اول
                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();

                    var headerCells = new Cell[headerKeys.Count];
                    for (int i = 0; i < headerKeys.Count; i++)
                        headerCells[i] = new Cell(headerKeys[i]);

                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                    converters = new Func<object?, Cell>[headerKeys.Count];

                    for (int i = 0; i < headerKeys.Count; i++)
                    {
                        var key = headerKeys[i];
                        var sample = batch[0][key];

                        converters[i] = sample == null ? _ => new Cell(string.Empty) :
                            sample is DateTime ? v => new Cell((DateTime?)v) :
                            sample is DateTimeOffset ? v => new Cell(((DateTimeOffset?)v)?.DateTime) :
                            sample is int ? v => new Cell((int?)v) :
                            sample is long ? v => new Cell((long?)v) :
                            sample is short ? v => new Cell(Convert.ToInt32(v)) :
                            sample is byte ? v => new Cell(Convert.ToInt32(v)) :
                            sample is double ? v => new Cell((double?)v) :
                            sample is float ? v => new Cell(Convert.ToDouble(v)) :
                            sample is decimal ? v => new Cell(Convert.ToDouble(v)) :
                            sample is bool ? v => new Cell((bool?)v) :
                            sample is Guid ? v => new Cell(v?.ToString()) :
                            sample is TimeSpan ? v => new Cell(v?.ToString()) :
                            v => new Cell(v?.ToString() ?? string.Empty);
                    }

                    rowCells = new Cell[headerKeys.Count];
                    headerWritten = true;
                }

                // Data
                foreach (var rowDict in batch)
                {
                    for (int i = 0; i < headerKeys!.Count; i++)
                    {
                        rowDict.TryGetValue(headerKeys[i], out var val);
                        rowCells![i] = converters![i](val);
                    }

                    await spreadsheet.AddRowAsync(rowCells!, cancellationToken);
                }

                offset += batch.Count;
            }

            await spreadsheet.FinishAsync(cancellationToken);
        }
    }
}