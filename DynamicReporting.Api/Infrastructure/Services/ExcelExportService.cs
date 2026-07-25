using ClosedXML.Excel;
using Microsoft.AspNetCore.Routing.Patterns;

namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     کلاس خروجی گرفتن با اکسل با پرفورمنس بالا که برای داده‌های حجیم بهینه شده است.
///     کمتری مصرف رم و پکیج SpreadCheetah
/// </summary>
public class ExcelExportService(IReportDataService reportDataService, IReportExportService exportService, IReportMetadataService metadataService, IReportQueryBuilder reportQueryBuilder) : IExportService
{
    public async Task ExportAsync(int reportDefinitionId, List<FilterCondition>? filtersList, Stream outputStream, SortableColumnDto sortColumn, CancellationToken cancellationToken = default)
    {
        const int batchSize = 6000;
        var stopAt = 200;
        if (filtersList != null && filtersList.Count != 0)
        {
            var (whereClause, parameters) = reportQueryBuilder.BuildWhereClause(filtersList);
            stopAt = await reportDataService.GetTotalCountAsync(reportDefinitionId, (whereClause, parameters));
        }
        using var tempStream = new MemoryStream();

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
            if (batch.Count == 0)
                break;

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

        tempStream.Position = 0;

        // استفاده از using برای مدیریت خودکار منابع
        using var package = new ExcelPackage(tempStream);

        // بررسی وجود Worksheet
        if (package.Workbook.Worksheets.Count == 0)
            throw new InvalidOperationException("هیچ برگه‌ای در فایل وجود ندارد");

        var worksheet = package.Workbook.Worksheets[0];

        // بررسی وجود داده
        if (worksheet.Dimension == null)
            return; // فایل خالی است

        // اعمال AutoFit
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // ذخیره در همان Stream
        await package.SaveAsync(cancellationToken);

        // کپی به خروجی
        tempStream.Position = 0;
        await tempStream.CopyToAsync(outputStream, cancellationToken);
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

    private void FitColumns(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        IXLWorksheet? worksheet = workbook.Worksheet(1);
        worksheet.Columns().AdjustToContents();
        workbook.Save();
    }
}