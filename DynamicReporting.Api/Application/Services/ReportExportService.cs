using System.Globalization;

namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcel0Async(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
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

        public async Task ExportToExcel1Async(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 6000;
            const int stopAt = 12000;

            var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);

            try
            {
                await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

                bool headerWritten = false;
                List<string>? headerKeys = null;
                // Func<object?, Cell>[]? converters = null;
                Cell[]? rowCells = null;

                int offset = 0;
                bool outerShouldStop = false;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();


                    // check stop BEFORE fetching next batch so we don't start another heavy query
                    if (offset > stopAt)
                    {
                        break;
                    }

                    var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, offset, batchSize);

                    if (batch.Count == 0)
                    {
                        break;
                    }

                    // Header + TypeMap only once
                    if (!headerWritten)
                    {
                        headerKeys = batch[0].Keys.ToList();

                        var headerCells = new Cell[headerKeys.Count];
                        for (int i = 0; i < headerKeys.Count; i++)
                            headerCells[i] = new Cell(headerKeys[i]);

                        await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                        //  converters = new Func<object?, Cell>[headerKeys.Count];

                        //     for (int i = 0; i < headerKeys.Count; i++)
                        {
                            // var sample = batch[0][headerKeys[i]]?.ToString();

                            //converters[i] = sample == null ? v => new Cell(string.Empty) :
                            //    sample is DateTime _ ? v => new Cell((DateTime?)v) :
                            //    sample is DateTimeOffset _ ? v => new Cell(((DateTimeOffset?)v)?.DateTime) :
                            //    sample is int _ ? v => new Cell((int?)v) :
                            //    sample is long _ ? v => new Cell((long?)v) :
                            //    sample is short _ ? v => new Cell(Convert.ToInt32(v)) :
                            //    sample is byte _ ? v => new Cell(Convert.ToInt32(v)) :
                            //    sample is double _ ? v => new Cell((double?)v) :
                            //    sample is float _ ? v => new Cell(Convert.ToDouble(v)) :
                            //    sample is decimal _ ? v => new Cell(Convert.ToDouble(v)) :
                            //    sample is bool _ ? v => new Cell((bool?)v) :
                            //    sample is Guid _ ? v => new Cell(v?.ToString()) :
                            //    sample is TimeSpan _ ? v => new Cell(v?.ToString()) :
                            //    v => new Cell(v?.ToString() ?? string.Empty);

                            //  converters[i] = sample.ToString();

                        }

                        rowCells = new Cell[headerKeys.Count];
                        headerWritten = true;
                    }

                    // Data: process row-by-row so we can stop mid-batch precisely
                    bool stopRequested = false;
                    foreach (var rowDict in batch)
                    {
                        // check cancellation token frequently
                        cancellationToken.ThrowIfCancellationRequested();

                        // if offset already past stopAt, request stop and break
                        if (offset >= stopAt)
                        {
                            stopRequested = true;
                            break;
                        }

                        for (int i = 0; i < headerKeys!.Count; i++)
                        {
                            rowDict.TryGetValue(headerKeys[i], out var val);
                            //  rowCells![i] = converters![i](val);
                            rowCells![i] = new Cell((val?.ToString()));
                        }

                        await spreadsheet.AddRowAsync(rowCells!, cancellationToken);

                        // increment offset per written row (important for precise stopping)
                        offset++;
                    }

                    if (stopRequested)
                    {
                        outerShouldStop = true;
                    }

                    if (outerShouldStop)
                    {
                        break;
                    }

                    // Note: we do NOT do offset += batch.Count here because we increment per row above.
                }

                // Try to finish normally and mark finishCalled = true only on success
                await spreadsheet.FinishAsync(cancellationToken);
            }
            finally
            {
                await spreadsheet.DisposeAsync();

            }
        }

        //two
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
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