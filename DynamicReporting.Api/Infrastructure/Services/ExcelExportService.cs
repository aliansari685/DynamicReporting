namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     کلاس خروجی گرفتن با اکسل با پرفورمنس بالا که برای داده‌های حجیم بهینه شده است.
///     کمتری مصرف رم و پکیج SpreadCheetah
/// </summary>
public class ExcelExportService(
    IReportDataService reportDataService,
    IReportExportService exportService,
    IReportMetadataService metadataService,
    IReportQueryBuilder reportQueryBuilder) : IExportService
{
    public async Task ExportAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream,
        SortableColumnDto sortColumn, CancellationToken cancellationToken = default)
    {
        const int batchSize = 6000;
        var stopAt = 200;
        if (filtersList != null && filtersList.Count != 0)
        {
            var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);

            stopAt = await reportDataService.GetTotalCountAsync(reportDefinitionId, (whereClause, parameters));

            if (stopAt <= 0)
            {
                //todo : ایجاد اکسل بدون دیتا
                throw new OperationCanceledException("دیتایی وجود ندارد");
            }
        }

        await using var spreadsheet =
            await Spreadsheet.CreateNewAsync(outputStream, cancellationToken: cancellationToken);
        await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);
        var headerWritten = false;

        //   List<string>? englishHeaders = null;
        List<string>? persianHeaders = null;
        Cell[]? rowCells = null;
        var fetchOffset = 0;
        var written = 0;
        while (written < stopAt)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //دیتای اصلی
            var data = await exportService.GetExportBatchAsync(reportDefinitionId, filtersList, fetchOffset, batchSize,
                sortColumn, cancellationToken);

            //اضافه کردن نام فارسی ستون ها به دیتا اصلی
            var batch = metadataService.GetDisplayNameColumn(data);
            if (batch.Count == 0) break;
            if (!headerWritten)
            {
                // دریافت همه کلیدها
                var allKeys = batch[0].Keys.ToList();

                // تقسیم کلیدها به دو نیمه
                var halfCount = allKeys.Count / 2;

                // نیمه اول = ستون‌های انگلیسی
                //    englishHeaders = allKeys.Take(halfCount).ToList();

                // نیمه دوم = ستون‌های فارسی
                persianHeaders = allKeys.Skip(halfCount).ToList();

                // ساخت هدره فارسی 
                var headerCells = persianHeaders.Select(key => new Cell(key)).ToList();
                await spreadsheet.AddRowAsync(headerCells.ToArray(), cancellationToken);
                rowCells = new Cell[allKeys.Count];
                headerWritten = true;
            }

            var rowsAllowed = Math.Min(batch.Count, stopAt - written);
            for (var r = 0; r < rowsAllowed; r++)
            {
                var rowDict = batch[r];
                var cellIndex = 0;

                //// ابتدا ستون‌های انگلیسی
                //foreach (var key in englishHeaders!)
                //{
                //    rowDict.TryGetValue(key, out var val);
                //    rowCells![cellIndex] = ConvertToCell(val);
                //    cellIndex++;
                //}

                // سپس ستون‌های فارسی
                foreach (var key in persianHeaders!)
                {
                    rowDict.TryGetValue(key, out var val);
                    rowCells![cellIndex] = ConvertToCell(val);
                    cellIndex++;
                }
                await spreadsheet.AddRowAsync(rowCells!, cancellationToken);
                written++;
            }
            fetchOffset += batch.Count;
        }
        await spreadsheet.FinishAsync(cancellationToken);
    }

    public async Task ExportWithAutoFitColumnsAsync(
      int reportDefinitionId,
      List<FilterCondition>? filtersList,
      Stream outputStream,
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
        // فایل ابتدا داخل این Stream ساخته می‌شود
        await using var excelStream = new MemoryStream();

        await using (var spreadsheet =
            await Spreadsheet.CreateNewAsync(excelStream, cancellationToken: cancellationToken))
        {
            await spreadsheet.StartWorksheetAsync("Report", token: cancellationToken);

            var headerWritten = false;

            List<string>? persianHeaders = null;
            Cell[]? rowCells = null;

            var fetchOffset = 0;
            var written = 0;

            while (written < stopAt)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = await exportService.GetExportBatchAsync(
                    reportDefinitionId,
                    filtersList,
                    fetchOffset,
                    batchSize,
                    sortColumn,
                    cancellationToken);

                var batch = metadataService.GetDisplayNameColumn(data);

                if (batch.Count == 0)
                    break;

                if (!headerWritten)
                {
                    var allKeys = batch[0].Keys.ToList();

                    var halfCount = allKeys.Count / 2;

                    persianHeaders = allKeys.Skip(halfCount).ToList();

                    var headerCells = persianHeaders
                        .Select(x => new Cell(x))
                        .ToArray();

                    await spreadsheet.AddRowAsync(headerCells, cancellationToken);

                    // فقط به تعداد ستون‌هایی که می‌نویسی
                    rowCells = new Cell[persianHeaders.Count];

                    headerWritten = true;
                }
                var rowsAllowed = Math.Min(batch.Count, stopAt - written);

                for (var r = 0; r < rowsAllowed; r++)
                {
                    var row = batch[r];

                    for (var i = 0; i < persianHeaders!.Count; i++)
                    {
                        row.TryGetValue(persianHeaders[i], out var value);

                        rowCells![i] = ConvertToCell(value);
                    }
                    await spreadsheet.AddRowAsync(rowCells!, cancellationToken);
                    written++;
                }
                fetchOffset += batch.Count;
            }
            await spreadsheet.FinishAsync(cancellationToken);
        }
        await SetAutoFitColumns(outputStream, cancellationToken, excelStream);
    }

    /// <summary>
    /// Set AutoFitColumns With EPPlus Package in MemoryStreaming
    /// </summary>
    /// <param name="outputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="excelStream"></param>
    /// <returns></returns>
    private async Task SetAutoFitColumns(Stream outputStream, CancellationToken cancellationToken,
        MemoryStream excelStream)
    {
        excelStream.Position = 0;

        using var package = new ExcelPackage(excelStream);

        var worksheet = package.Workbook.Worksheets[0];

        worksheet.Cells.AutoFitColumns();

        outputStream.SetLength(0);

        await package.SaveAsAsync(outputStream, cancellationToken);

        outputStream.Position = 0;
    }

    // متد کمکی برای تبدیل مقدار به Cell
    private Cell ConvertToCell(object? val)
    {
        return val switch
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

    //private void FitColumns(string filePath)
    //{
    //    using var workbook = new XLWorkbook(filePath);
    //    var worksheet = workbook.Worksheet(1);
    //    worksheet.Columns().AdjustToContents();
    //    workbook.Save();
    //}
}