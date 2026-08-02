namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     خروجی CSV با حداقل مصرف حافظه و حداکثر سرعت
/// </summary>
public class CsvExportService(
    IReportDataService reportDataService,
    IReportExportService exportService,
    IReportMetadataService metadataService,
    IReportQueryBuilder reportQueryBuilder) : IExportService
{
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

        await using var writer = new StreamWriter(
            outputStream,
            new UTF8Encoding(true),
            64 * 1024,
            true);

        var headerWritten = false;
        List<string>? persianHeaders = null;

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

                var half = allKeys.Count / 2;

                persianHeaders = allKeys.Skip(half).ToList();

                for (var i = 0; i < persianHeaders.Count; i++)
                {
                    if (i > 0)
                        await writer.WriteAsync(',');

                    WriteEscaped(writer, persianHeaders[i]);
                }

                await writer.WriteLineAsync();

                headerWritten = true;
            }

            var rowsAllowed = Math.Min(batch.Count, stopAt - written);

            for (var r = 0; r < rowsAllowed; r++)
            {
                var row = batch[r];

                for (var c = 0; c < persianHeaders!.Count; c++)
                {
                    if (c > 0)
                        await writer.WriteAsync(',');

                    row.TryGetValue(persianHeaders[c], out var value);

                    WriteValue(writer, value);
                }

                await writer.WriteLineAsync();

                written++;
            }

            fetchOffset += batch.Count;
        }

        await writer.FlushAsync(cancellationToken);

        return true;
    }

    private static void WriteValue(StreamWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                return;

            case string s:
                WriteEscaped(writer, s);
                return;

            case DateTime dt:
                WriteEscaped(writer,
                    dt.ToString(CultureInfo.CurrentCulture));
                return;

            case DateTimeOffset dto:
                WriteEscaped(writer,
                    dto.DateTime.ToString(CultureInfo.CurrentCulture));
                return;

            case bool b:
                writer.Write(b ? "True" : "False");
                return;

            case int i:
                writer.Write(i);
                return;

            case long l:
                writer.Write(l);
                return;

            case short s:
                writer.Write(s);
                return;

            case byte b:
                writer.Write(b);
                return;

            case float f:
                writer.Write(f.ToString(CultureInfo.CurrentCulture));
                return;

            case double d:
                writer.Write(d.ToString(CultureInfo.CurrentCulture));
                return;

            case decimal m:
                writer.Write(m.ToString(CultureInfo.CurrentCulture));
                return;

            case IFormattable formattable:
                WriteEscaped(writer,
                    formattable.ToString(null, CultureInfo.CurrentCulture));
                return;

            default:
                WriteEscaped(writer, value.ToString());
                return;
        }
    }

    /// <summary>
    ///     Escape طبق استاندارد RFC4180
    /// </summary>
    private static void WriteEscaped(StreamWriter writer, string? text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var needsQuote = false;

        for (var i = 0; i < text.Length; i++)
            switch (text[i])
            {
                case ',':
                case '"':
                case '\r':
                case '\n':
                    needsQuote = true;
                    i = text.Length;
                    break;
            }

        if (!needsQuote)
        {
            writer.Write(text);
            return;
        }

        writer.Write('"');

        foreach (var ch in text)
            if (ch == '"')
                writer.Write("\"\"");
            else
                writer.Write(ch);

        writer.Write('"');
    }
}