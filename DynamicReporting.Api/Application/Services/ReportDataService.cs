using System.Diagnostics;
using System.Globalization;

namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(ShopTestDbContext dbContext, ISqlQueryExecutor sqlExecutor, IReportQueryBuilder queryBuilder, IMemoryCache memoryCache) : IReportDataService
{
    public async Task<PagedResult<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId, int page = 1, int take = 10)
    {
        if (page < 1) page = 1;
        if (take <= 0) take = 10;

        var report = await GetReportDefinition(reportDefinitionId);

        // کوئری داده برای پجینیشن
        var dataSql = queryBuilder.BuildPagedQuery(report, page, take);
        var data = await sqlExecutor.ExecuteAsync(dataSql);

        //بدست اوردن تعداد کل ردیف ها
        var totalCount = await GetTotalCountAsync(reportDefinitionId);

        return new PagedResult<Dictionary<string, object?>>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            Take = take,
        };
    }

    public async Task<int> GetTotalCountAsync(int reportDefinitionId, ReportDefinition? definition = null)
    {

        //2026-02-19 13:34:30.013 +03:30 [ERR] BuildCountQuery: took 8 ms
        // 2026-02-19 13:34:31.019 +03:30 [ERR] ExecuteScalarAsync: took 931 ms
        // 2026-02-19 13:34:31.023 +03:30 [ERR] GetTotalCountAsync(3) took 3844 ms
        // 2026-02-19 13:34:36.356 +03:30 [ERR] After Fill Excel: + 3685
        // 2026-02-19 13:34:36.413 +03:30 [ERR] Finish: + 9.2385226

        return await TimeLogger.TimeAsync(async () =>
            {
                var report = definition ?? await GetReportDefinition(reportDefinitionId);

                var cacheKey = $"report:{reportDefinitionId}:count";

                var totalCount = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                    string countSql = "";

                    TimeLogger.Time(() =>
                 countSql = queryBuilder.BuildCountQuery(report), "BuildCountQuery:");

                    int tmp = 0;

                    await TimeLogger.TimeAsync(async () =>
                        tmp = await sqlExecutor.ExecuteScalarAsync(countSql), "ExecuteScalarAsync:");

                    return tmp;
                });

                return totalCount;

            }, $"GetTotalCountAsync({reportDefinitionId})");
    }

    public async Task<List<Dictionary<string, object?>>> GetExportBatchAsync(int reportDefinitionId, int offset, int take)
    {
        if (take <= 0)
            throw new ArgumentException("تعداد ردیف ها باید بزرگتر از 0 باشد");

        var report = await GetReportDefinition(reportDefinitionId);

        //var sql = queryBuilder.BuildQuery(report, offset, take);
        var sql = "SELECT TOP (108000)\r\n[Customers].[FullName] AS [Customers_FullName], [Customers].[City] AS [Customers_City], [Customers].[Country] AS [Customers_Country], [Orders].[OrderDate] AS [Orders_OrderDate], [Orders].[Status] AS [Orders_Status], [Orders].[TotalAmount] AS [Orders_TotalAmount], [OrderItems].[Quantity] AS [OrderItems_Quantity], [OrderItems].[Total] AS [OrderItems_Total], [Products].[ProductName] AS [Products_ProductName], [Products].[Category] AS [Products_Category], [Products].[Price] AS [Products_Price], [Suppliers].[SupplierName] AS [Suppliers_SupplierName], [Suppliers].[Country] AS [Suppliers_Country]\r\nFROM [Customers]\r\nLEFT JOIN [Orders] ON [Orders].[CustomerId] = [Customers].[CustomerId]\r\nLEFT JOIN [OrderItems] ON [OrderItems].[OrderId] = [Orders].[OrderId]\r\nLEFT JOIN [Products] ON [Products].[ProductId] = [OrderItems].[ProductId]\r\nLEFT JOIN [Suppliers] ON [Suppliers].[SupplierId] = [Products].[SupplierId]\r\n\r\nORDER BY (SELECT NULL)";

        var batch = await sqlExecutor.ExecuteAsync(sql);
        return batch;
    }

    /// <summary>
    /// بدست اوردن ReportDefinition از دیتابیس با شناسه داده شده
    /// </summary>
    /// <param name="reportDefinitionId">شناسه گزارش</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private async Task<ReportDefinition> GetReportDefinition(int reportDefinitionId)
    {
        var report = await dbContext.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        return report ?? throw new KeyNotFoundException($"گزارش با شناسه {reportDefinitionId} وجود ندارد.");
    }
}