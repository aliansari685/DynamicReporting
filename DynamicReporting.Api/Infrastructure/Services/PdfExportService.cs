namespace DynamicReporting.Api.Infrastructure.Services;

//todo :test and run
public class PdfExportService(
    IReportDataService reportDataService,
    IReportExportService exportService,
    IReportMetadataService metadataService,
    IReportQueryBuilder reportQueryBuilder) : IExportService
{
    /// <summary>
    ///       =====================================================================
    /// NOTE:
    /// این پیاده‌سازی صرفاً جهت نمایش قابلیت Export به PDF است.
    /// برخلاف Excel (SpreadCheetah)، کتابخانه QuestPDF از Streaming
    /// برای تولید جدول پشتیبانی نمی‌کند و باید کل داده‌ها ابتدا
    /// در حافظه جمع‌آوری شوند.
    /// بنابراین برای گزارش‌های بسیار بزرگ مناسب نیست و فقط
    /// جهت دمو و نمایش توانایی پروژه استفاده شده است.
    /// =====================================================================
    /// </summary>
    /// <param name="reportDefinitionId"></param>
    /// <param name="filtersList"></param>
    /// <param name="outputStream"></param>
    /// <param name="sortColumn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExportAsync(
        int reportDefinitionId,
        List<FilterCondition>? filtersList,
        Stream outputStream,
        SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default)
    {

        const int batchSize = 6000;

        var stopAt = 200;

        if (filtersList is { Count: > 0 })
        {
            var (whereClause, parameters) =
                reportQueryBuilder.BuildWhereClause(filtersList);

            stopAt = await reportDataService.GetTotalCountAsync(
                reportDefinitionId,
                (whereClause, parameters));

            if (stopAt <= 0)
                return false;
        }

        List<string>? headers = null;

        var rows = new List<object?[]>();

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

            if (headers == null)
            {
                var allKeys = batch[0].Keys.ToList();

                var half = allKeys.Count / 2;

                headers = allKeys.Skip(half).ToList();
            }

            var rowsAllowed = Math.Min(batch.Count, stopAt - written);

            for (var r = 0; r < rowsAllowed; r++)
            {
                var row = batch[r];

                var values = new object?[headers.Count];

                for (var i = 0; i < headers.Count; i++)
                {
                    row.TryGetValue(headers[i], out var value);

                    values[i] = value;
                }

                rows.Add(values);

                written++;
            }

            fetchOffset += batch.Count;
        }

        var fontSize = headers!.Count switch
        {
            <= 8 => 10,
            <= 15 => 8,
            <= 25 => 7,
            <= 35 => 6,
            <= 50 => 5,
            _ => 4
        };

        var margin = headers.Count switch
        {
            <= 15 => 15,
            <= 30 => 8,
            _ => 3
        };

        Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    if (headers.Count > 8)
                        page.PageColor(Colors.White);

                    page.Margin(margin);

                    page.DefaultTextStyle(style =>
                        style
                            .FontFamily("B Nazanin")
                            .FontSize(fontSize));

                    page.Header()
                        .AlignCenter()
                        .Text("گزارش")
                        .Bold()
                        .FontFamily("B Nazanin")
                        .FontSize(fontSize + 4);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var _ in headers)
                                    columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                foreach (var column in headers)
                                {
                                    header.Cell()
                                        .Background(Colors.Grey.Lighten2)
                                        .Border(0.5f)
                                        .Padding(2)
                                        .AlignCenter()
                                        .Text(column)
                                        .Bold()
                                        .FontFamily("B Nazanin")
                                        .FontSize(fontSize);
                                }
                            });

                            foreach (var value in rows.SelectMany(row => row))
                            {
                                table.Cell()
                                    .Border(0.25f)
                                    .Padding(1)
                                    .Text(value?.ToString() ?? string.Empty)
                                    .FontFamily("B Nazanin")
                                    .FontSize(fontSize);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");

                            text.CurrentPageNumber();

                            text.Span(" / ");

                            text.TotalPages();
                        });
                });
            })
            .GeneratePdf(outputStream);

        return true;
    }
}