using System.Globalization;

namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        //2026-02-26 13:38:22.958 +03:30 [INF] Finish: + 2.9568005
        // 2026-02-26 13:38:32.936 +03:30 [INF] Finish: + 0.9431704
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {

            int totalCount = await reportDataService.GetTotalCountAsync(reportDefinitionId);

            int batchSize = Math.Clamp(totalCount / 100, 5000, 15000);
            //  int batchSize = 6000;

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null;

            for (int offset = 0; offset < 12000 /*totalCount*/; offset += batchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

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

        //2026-02-26 13:35:49.518 +03:30 [INF] Finish: + 2.3258424
        // 2026-02-26 13:36:07.661 +03:30 [INF] Finish: + 1.0751972
        public async Task ExportToExcel1Async(int reportDefinitionId, Stream outputStream,
            CancellationToken cancellationToken = default)
        {
            const int batchSize = 6000;
            const int stopAt = 12000;

            var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            try
            {
                await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

                var headerWritten = false;
                List<string>? headerKeys = null;
                Cell[]? rowCells = null;

                var offset = 0;
                var outerShouldStop = false;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (offset > stopAt) break;

                    var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                    if (batch.Count == 0) break;

                    if (!headerWritten)
                    {
                        headerKeys = batch[0].Keys.ToList();

                        var headerCells = new Cell[headerKeys.Count];
                        for (var i = 0; i < headerKeys.Count; i++)
                            headerCells[i] = new Cell(headerKeys[i]);

                        await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                        rowCells = new Cell[headerKeys.Count];
                        headerWritten = true;
                    }

                    var stopRequested = false;
                    foreach (var rowDict in batch)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (offset >= stopAt)
                        {
                            stopRequested = true;
                            break;
                        }

                        for (var i = 0; i < headerKeys!.Count; i++)
                        {
                            rowDict.TryGetValue(headerKeys[i], out var val);
                            rowCells![i] = new Cell(val?.ToString());
                        }

                        await spreadsheet.AddRowAsync(rowCells!, cancellationToken);

                        offset++;
                    }

                    if (stopRequested) outerShouldStop = true;

                    if (outerShouldStop) break;
                }

                await spreadsheet.FinishAsync(cancellationToken);
            }
            finally
            {
                await spreadsheet.DisposeAsync();
            }
        }

        //two
        //  2026-02-26 13:29:29.871 +03:30 [INF] Finish: + 2.898683
        //  2026-02-26 13:29:59.318 +03:30 [INF] Finish: + 0.8320888
        public async Task ExportToExcel2Async(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 6000;
            const int stopAt = 12000;

            // create spreadsheet and worksheet
            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);
            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null;
            Cell[]? rowCells = null;

            int fetchOffset = 0; // offset used when fetching from DB (in rows)
            int written = 0;     // number of rows already written to the sheet


            while (written < stopAt)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // fetch next batch from DB using fetchOffset (batch-based)
                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, fetchOffset, batchSize);

                if (batch.Count == 0)
                    break;

                // write header once
                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();
                    var headerCells = new Cell[headerKeys.Count];
                    for (int i = 0; i < headerKeys.Count; i++)
                        headerCells[i] = new Cell(headerKeys[i]);

                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                    rowCells = new Cell[headerKeys.Count];
                    headerWritten = true;
                }

                // determine how many rows from this batch we actually need to write
                int rowsAllowed = Math.Min(batch.Count, stopAt - written);

                for (int r = 0; r < rowsAllowed; r++)
                {
                    var rowDict = batch[r];

                    // fill rowCells in-place
                    for (int i = 0; i < headerKeys!.Count; i++)
                    {
                        rowDict.TryGetValue(headerKeys[i], out var val);

                        // basic type-aware conversion to Cell to avoid unnecessary string allocations
                        rowCells![i] = val == null ? new Cell(string.Empty) :
                            val is DateTime dt ? new Cell(dt.ToString(CultureInfo.CurrentCulture)) :
                            val is DateTimeOffset dto ? new Cell(dto.DateTime.ToString(CultureInfo.CurrentCulture)) :
                            val is int iv ? new Cell(iv) :
                            val is long lv ? new Cell(lv) :
                            val is double dv ? new Cell(dv) :
                            val is float fv ? new Cell(Convert.ToDouble(fv)) :
                            val is decimal dec ? new Cell(Convert.ToDouble(dec)) :
                            val is bool bv ? new Cell(bv) : new Cell(val.ToString() ?? string.Empty);
                    }

                    // add the row (re-using the same array is fine as AddRowAsync should read synchronously)
                    await spreadsheet.AddRowAsync(rowCells!, cancellationToken);

                    written++;
                }

                // advance fetchOffset by how many rows we fetched (not how many we've written in total)
                fetchOffset += batch.Count;
            }

            await spreadsheet.FinishAsync(cancellationToken);
        }
    }
}