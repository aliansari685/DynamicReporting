namespace Tests.Application.Tests.Services;

public class ReportDataServiceTests
{
    private readonly Mock<ISqlQueryExecutor> _sqlExecutorMock = new();
    private readonly Mock<IReportQueryBuilder> _queryBuilderMock = new();

    private ReportDataService CreateService(ShopTestDbContext context)
        => new(context, _sqlExecutorMock.Object, _queryBuilderMock.Object);


    /// <summary>
    /// تست خطا زمانی که گزارش وجود ندارد
    /// </summary>
    [Fact(DisplayName = "GetReportDataAsync - Throws when report not found")]
    public async Task GetReportDataAsync_ShouldThrow_WhenReportDoesNotExist()
    {
        // ---------- Arrange ----------
        await using var context = DbContextFactory.Create();
        var service = CreateService(context);

        // ---------- Act & Assert ----------
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetReportDataAsync(999));

        Assert.Contains("گزارش با شناسه 999 وجود ندارد", ex.Message);
    }

    /// <summary>
    /// تست نرمال‌سازی مقادیر نامعتبر page و take
    /// </summary>
    [Theory(DisplayName = "GetReportDataAsync - Normalizes invalid page and take values")]
    [InlineData(0, 0, 1, 10)]
    [InlineData(-5, -10, 1, 10)]
    public async Task GetReportDataAsync_ShouldNormalizeInvalidPageAndTake(
        int page,
        int take,
        int expectedPage,
        int expectedTake)
    {
        // ---------- Arrange ----------
        await using var context = DbContextFactory.Create();

        var report = new ReportDefinition
        {
            Id = 3,
            Name = "Normalize Test",
            BaseTable = "OrderItems",
            SelectedColumns =
            [
                new SelectedColumn { Table = "Customers", Column = "FullName" },
                new SelectedColumn { Table = "Customers", Column = "City" },
                new SelectedColumn { Table = "Customers", Column = "Country" },
                new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                new SelectedColumn { Table = "Orders", Column = "Status" },
                new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                new SelectedColumn { Table = "OrderItems", Column = "Total" },
                new SelectedColumn { Table = "Products", Column = "ProductName" },
                new SelectedColumn { Table = "Products", Column = "Category" },
                new SelectedColumn { Table = "Products", Column = "Price" },
                new SelectedColumn { Table = "Suppliers", Column = "SupplierName" },
                new SelectedColumn { Table = "Suppliers", Column = "Country" }
            ]
        };
        context.ReportDefinitions.Add(report);
        await context.SaveChangesAsync();

        _queryBuilderMock.Setup(q => q.BuildQuery(report, expectedPage, expectedTake))
            .Returns("QUERY");

        _queryBuilderMock.Setup(q => q.BuildCountQuery(report))
            .Returns("COUNT");

        _sqlExecutorMock.Setup(s =>
                s.ExecuteAsync("QUERY", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _sqlExecutorMock.Setup(s =>
                s.ExecuteScalarAsync("COUNT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var service = CreateService(context);

        // ---------- Act ----------
        var result = await service.GetReportDataAsync(report.Id, page, take);

        // ---------- Assert ----------
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedTake, result.Take);
    }
}
