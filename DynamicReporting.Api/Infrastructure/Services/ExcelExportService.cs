namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     کلاس خروجی گرفتن با اکسل با پرفورمنس بالا که برای داده‌های حجیم بهینه شده است.
///     کمتری مصرف رم و پکیج SpreadCheetah
/// </summary>
public class ExcelExportService(
    IReportDataService reportDataService,
    IReportExportService exportService,
    IReportQueryBuilder reportQueryBuilder) : IExportService
{
    public async Task ExportAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream,
        SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default)
    {
        const int batchSize = 6000;
        var stopAt = 200;

        if (filtersList != null && filtersList.Count != 0)
        {
            var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);
            stopAt = await reportDataService.GetTotalCountAsync(reportDefinitionId, (whereClause, parameters));
        }

        await using var spreadsheet =
            await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);
        await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

        var headerWritten = false;
        List<string>? headerKeys = null;
        Cell[]? rowCells = null;

        var fetchOffset = 0;
        var written = 0;

        while (written < stopAt)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = await exportService.GetExportBatchAsync(reportDefinitionId, filtersList, fetchOffset,
                batchSize, sortColumn, cancellationToken);

            if (batch.Count == 0)
                break;

            if (!headerWritten)
            {
                headerKeys = batch[0].Keys.ToList();
                var headerCells = new Cell[headerKeys.Count];
                for (var i = 0; i < headerKeys.Count; i++) headerCells[i] = new Cell(headerKeys[i]);

                await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                rowCells = new Cell[headerKeys.Count];
                headerWritten = true;
            }

            var rowsAllowed = Math.Min(batch.Count, stopAt - written);

            for (var r = 0; r < rowsAllowed; r++)
            {
                var rowDict = batch[r];

                for (var i = 0; i < headerKeys!.Count; i++)
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