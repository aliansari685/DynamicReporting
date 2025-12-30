namespace Tests.Application.Tests.Services;

public class ReportDataServiceTests
{
    private readonly Mock<ISqlQueryExecutor> _sqlExecutorMock = new();
    private readonly Mock<IReportQueryBuilder> _queryBuilderMock = new();

    private ReportDataService CreateService(ShopTestDbContext context)
        => new(context, _sqlExecutorMock.Object, _queryBuilderMock.Object);

    /// <summary>
    /// تست برگرداندن داده گزارش به صورت PagedResult
    /// </summary>
    [Fact(DisplayName = "GetReportDataAsync - Returns PagedResult successfully")]
    public async Task GetReportDataAsync_ShouldReturnPagedResult_WhenReportExists()
    {
        // ---------- Arrange ----------
        await using var context = DbContextFactory.Create();

        var report = new ReportDefinition
        {
            Id = 1,
            Name = "Test Report",
            BaseTable = "Orders",
            SelectedColumns = null
        };
        context.ReportDefinitions.Add(report);
        await context.SaveChangesAsync();

        var fakeData = new List<Dictionary<string, object?>>
        {
            new() { ["Col1"] = "Value1" }
        };

        _queryBuilderMock.Setup(q => q.BuildQuery(report, 1, 10))
            .Returns("SELECT * FROM Dummy");

        _queryBuilderMock.Setup(q => q.BuildCountQuery(report))
            .Returns("SELECT COUNT(*) FROM Dummy");

        _sqlExecutorMock.Setup(s =>
                s.ExecuteAsync("SELECT * FROM Dummy", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeData);

        _sqlExecutorMock.Setup(s =>
                s.ExecuteScalarAsync("SELECT COUNT(*) FROM Dummy", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = CreateService(context);

        // ---------- Act ----------
        var result = await service.GetReportDataAsync(report.Id);

        // ---------- Assert ----------
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.Take);
        Assert.Single(result.Data);
        Assert.Equal("Value1", result.Data.First()["Col1"]);
    }

    /// <summary>
    /// تست صحت صفحه‌بندی برای صفحه دوم
    /// </summary>
    [Fact(DisplayName = "GetReportDataAsync - Returns correct data for page 2")]
    public async Task GetReportDataAsync_ShouldReturnCorrectDataForPage2()
    {
        // ---------- Arrange ----------
        await using var context = DbContextFactory.Create();

        //todo : init requred member
        var report = new ReportDefinition { Id = 2, Name = "Paged Report" };
        context.ReportDefinitions.Add(report);
        await context.SaveChangesAsync();

        var allData = Enumerable.Range(1, 15)
            .Select(i => new Dictionary<string, object?> { ["Col1"] = $"Value{i}" })
            .ToList();

        _queryBuilderMock.Setup(q => q.BuildQuery(report, 2, 10))
            .Returns("PAGE_2_QUERY");

        _queryBuilderMock.Setup(q => q.BuildCountQuery(report))
            .Returns("COUNT_QUERY");

        _sqlExecutorMock.Setup(s =>
                s.ExecuteAsync("PAGE_2_QUERY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(allData.Skip(10).Take(10).ToList());

        _sqlExecutorMock.Setup(s =>
                s.ExecuteScalarAsync("COUNT_QUERY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(allData.Count);

        var service = CreateService(context);

        // ---------- Act ----------
        var result = await service.GetReportDataAsync(report.Id, page: 2, take: 10);
        if (result != null)
        {
            // ---------- Assert ----------
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(10, result.Take);
            Assert.Equal(5, result.Data.Count);

            Assert.Equal("Value11", result.Data.First()["Col1"]);
            Assert.Equal("Value15", result.Data.Last()["Col1"]);
        }
    }

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

        var report = new ReportDefinition { Id = 3, Name = "Normalize Test" };
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
