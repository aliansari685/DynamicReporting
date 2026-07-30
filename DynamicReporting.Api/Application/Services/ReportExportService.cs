namespace DynamicReporting.Api.Application.Services;

public class ReportExportService(
    IServiceResolver serviceProvider,
    IReportQueryBuilder reportQueryBuilder,
    IReportValidation reportValidation,
    IMemoryCache memoryCache,
    IReportMetadataService metadataService) : IReportExportService
{
    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId,
        List<FilterCondition>? filtersList, int offset, int take, SortableColumnDto sortColumn,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await metadataService.GetReportDefinitionAsync(reportDefinitionId);

        if (filtersList != null)
            reportValidation.ValidateFilteringColumn(report, filtersList);

        var cacheKey = BuildCacheKey(reportDefinitionId, filtersList, offset, take, sortColumn);

        return (await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var (whereClause, parameters) =
                reportQueryBuilder.BuildWhereClause(filtersList);

            var sql = reportQueryBuilder.BuildQuery(
                report,
                whereClause,
                offset,
                take,
                sortColumn);

            var executor =
                serviceProvider.GetExecutorService(ServiceResolver.ExecutorType.AdoNet);

            return await executor.ExecuteAsync(sql, parameters, cancellationToken);
        }))!;
    }

    public async Task SetAutoFitColumnsWithPathAsync(string path)
    {
        var fullPathCombine = Path.Combine(Directory.GetCurrentDirectory(), path);

        using var package = new ExcelPackage(new FileInfo(fullPathCombine));

        using var worksheet = package.Workbook.Worksheets[0];
        if (worksheet?.Dimension == null)
            throw new InvalidOperationException("فایل اکسل خراب است");

        worksheet.Cells[worksheet.Dimension.Address]?.AutoFitColumns();

        await package.SaveAsync();
    }

    public async Task<MemoryStream> SetAutoFitColumnsWithStreamAsync(MemoryStream stream, CancellationToken cancellationToken)
    {
        using var package = new ExcelPackage(stream);
        using var worksheet = package.Workbook.Worksheets[0];
        if (worksheet?.Dimension == null)
            throw new InvalidOperationException("فایل اکسل معتبر نیست.");
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        var output = new MemoryStream();
        output.SetLength(0);
        await package.SaveAsAsync(output, cancellationToken);
        output.Position = 0;
        stream.Position = 0;
        stream.SetLength(0);
        stream.Capacity = 0;
        await stream.DisposeAsync();
        return output;
    }


    private static string BuildCacheKey(
        int reportDefinitionId,
        List<FilterCondition>? filters,
        int offset,
        int take,
        SortableColumnDto sortColumn)
    {
        var builder = new StringBuilder();

        builder.Append("ExportBatch|")
            .Append(reportDefinitionId)
            .Append('|')
            .Append(offset)
            .Append('|')
            .Append(take)
            .Append('|')
            .Append(JsonConvert.SerializeObject(sortColumn))
            .Append('|')
            .Append(JsonConvert.SerializeObject(filters));

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())));
    }

}