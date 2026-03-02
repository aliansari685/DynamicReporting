namespace DynamicReporting.Api.Application.Services
{
    public class ReportExportService(IReportDataService reportDataService) : IReportExportService
    {
        public async Task ExportToExcelAsync(int reportDefinitionId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            const int batchSize = 6000;
            const int stopAt = 12000;

            await using var spreadsheet = await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);
            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            bool headerWritten = false;
            List<string>? headerKeys = null;
            Cell[]? rowCells = null;

            int fetchOffset = 0;
            int written = 0;

            while (written < stopAt)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await reportDataService.GetExportBatchAsync(reportDefinitionId, fetchOffset, batchSize, cancellationToken);

                if (batch.Count == 0)
                    break;

                if (!headerWritten)
                {
                    headerKeys = batch[0].Keys.ToList();
                    var headerCells = new Cell[headerKeys.Count];
                    for (int i = 0; i < headerKeys.Count; i++)
                    {
                        headerCells[i] = new Cell(headerKeys[i]);
                    }

                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                    rowCells = new Cell[headerKeys.Count];
                    headerWritten = true;
                }

                int rowsAllowed = Math.Min(batch.Count, stopAt - written);

                for (int r = 0; r < rowsAllowed; r++)
                {
                    var rowDict = batch[r];

                    for (int i = 0; i < headerKeys!.Count; i++)
                    {
                        rowDict.TryGetValue(headerKeys[i], out var val);

                        rowCells![i] = val switch
                        {
                            null => new Cell(string.Empty),
                            DateTime dt => new Cell(dt.ToString(CultureInfo.CurrentCulture)),
                            DateTimeOffset dto => new Cell(dto.DateTime.ToString(CultureInfo.CurrentCulture)),
                            int iv => new Cell(iv),
                            long lv => new Cell(lv),
                            double dv => new Cell(dv),
                            float fv => new Cell(Convert.ToDouble(fv)),
                            decimal dec => new Cell(Convert.ToDouble(dec)),
                            bool bv => new Cell(bv),
                            _ => new Cell(val.ToString() ?? string.Empty)
                        };
                    }

                    await spreadsheet.AddRowAsync(rowCells!, cancellationToken);

                    written++;
                }
                fetchOffset += batch.Count;
            }
            await spreadsheet.FinishAsync(cancellationToken);
        }
    }
}