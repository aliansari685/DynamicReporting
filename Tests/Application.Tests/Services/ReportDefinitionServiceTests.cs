namespace Tests.Application.Tests.Services;

public class ReportDefinitionServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IRepository<ReportDefinition>> _repoMock;
    private readonly Mock<IBaseTableResolver> _baseTableResolverMock;
    private readonly ReportDefinitionService _service;

    public ReportDefinitionServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<IRepository<ReportDefinition>>();
        _baseTableResolverMock = new Mock<IBaseTableResolver>();

        _uowMock.Setup(u => u.Repository<ReportDefinition>()).Returns(_repoMock.Object);

        _service = new ReportDefinitionService(_uowMock.Object, _baseTableResolverMock.Object);
    }

    #region GetByIdAsync

    /// <summary>
    ///     تست برگرداندن گزارش با شناسه موجود
    /// </summary>
    [Fact(DisplayName = "GetByIdAsync - Returns report when found")]
    public async Task GetByIdAsync_ShouldReturnReport_WhenExists()
    {
        var report = new ReportDefinition
        {
            Id = 1,
            Name = "Test",
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
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(report, result);
    }

    /// <summary>
    ///     تست استثنا زمانی که گزارش با شناسه داده شده وجود ندارد
    /// </summary>
    [Fact(DisplayName = "GetByIdAsync - Throws NullReferenceException when not found")]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ReportDefinition?)null);

        var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetByIdAsync(99));
        Assert.Contains("شناسه وجود ندارد", ex.Message);
    }

    #endregion

    #region GetAll / GetAllToListAsync / GetByPropertyAsync

    /// <summary>
    ///     تست برگرداندن همه گزارش‌ها بصورت IEnumerable
    /// </summary>
    [Fact(DisplayName = "GetAll - Returns all reports")]
    public void GetAll_ShouldReturnAllReports()
    {
        var reports = new List<ReportDefinition>
        {
            new()
            {
                Id = 1,
                Name = "Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ]
            },
            new()
            {
                Id = 2,
                Name = "Paged Report",
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
            }
        }.AsQueryable(); // ← این مهمه

        _repoMock.Setup(r => r.GetAll()).Returns(reports);

        var result = _service.GetAll();

        Assert.Equal(reports.ToList(), result);
    }

    /// <summary>
    ///     تست برگرداندن همه گزارش‌ها بصورت List Async
    /// </summary>
    [Fact(DisplayName = "GetAllToListAsync - Returns all reports as list")]
    public async Task GetAllToListAsync_ShouldReturnAllReportsAsList()
    {
        var reports = new List<ReportDefinition>
        {
            new()
            {
                Id = 1,
                Name = "Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ]
            },
            new()
            {
                Id = 2,
                Name = "Paged Report",
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
            }
        };
        _repoMock.Setup(r => r.GetAllToListAsync()).ReturnsAsync(reports);

        var result = await _service.GetAllToListAsync();

        Assert.Equal(reports, result);
    }

    /// <summary>
    ///     تست برگرداندن یک گزارش بر اساس شرط خاص
    /// </summary>
    [Fact(DisplayName = "GetByPropertyAsync - Returns report matching predicate")]
    public async Task GetByPropertyAsync_ShouldReturnReportMatchingPredicate()
    {
        var report = new ReportDefinition
        {
            Id = 1,
            Name = "Customer Orders Report",
            BaseTable = "Customers",
            SelectedColumns =
            [
                new SelectedColumn { Table = "Customers", Column = "FullName" },
                new SelectedColumn { Table = "Customers", Column = "City" },
                new SelectedColumn { Table = "Customers", Column = "Country" },
                new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                new SelectedColumn { Table = "Orders", Column = "Status" },
                new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                new SelectedColumn { Table = "OrderItems", Column = "Total" }
            ]
        };
        _repoMock.Setup(r => r.GetByPropertyAsync(It.IsAny<Expression<Func<ReportDefinition, bool>>>()))
            .ReturnsAsync(report);

        var result = await _service.GetByPropertyAsync(r => r.Id == 1);

        Assert.Equal(report, result);
    }

    #endregion

    #region CreateAsync

    /// <summary>
    ///     تست ایجاد گزارش جدید و Resolve شدن BaseTable بر اساس SelectedColumns
    /// </summary>
    [Fact(DisplayName = "CreateAsync - Adds report and resolves BaseTable")]
    public async Task CreateAsync_ShouldAddReport_AndResolveBaseTable()
    {
        // ---------- Arrange ----------
        var selectedColumns = new List<SelectedColumn>
        {
            new() { Table = "Tbl1", Column = "Col1" },
            new() { Table = "Tbl2", Column = "Col2" }
        };

        var dto = new ReportDefinitionDto
        {
            Name = "Test Report", // فراموش نکنید Name را اضافه کنید
            IsDefault = false,
            SelectedColumns = selectedColumns
        };

        _baseTableResolverMock
            .Setup(b => b.Resolve(It.IsAny<List<SelectedColumn>>()))
            .Returns("ResolvedTable");

        // ---------- Act ----------
        await _service.CreateAsync(dto);

        // ---------- Assert ----------
        _repoMock.Verify(r =>
                r.Add(It.Is<List<ReportDefinition>>(list =>
                    list.Count == 1 &&
                    list[0].BaseTable == "ResolvedTable" &&
                    AreEqual(list[0].SelectedColumns!, selectedColumns)
                )),
            Times.Once);

        _uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    // متد کمکی برای مقایسه لیست‌ها
    private static bool AreEqual(List<SelectedColumn>? list1, List<SelectedColumn>? list2)
    {
        if (list1 == null && list2 == null) return true;
        if (list1 == null || list2 == null) return false;
        if (list1.Count != list2.Count) return false;

        return list1.All(item1 =>
            list2.Any(item2 =>
                item1.Table == item2.Table &&
                item1.Column == item2.Column
            )
        );
    }

    /// <summary>
    ///     تست Integration:
    ///     وقتی گزارش جدید IsDefault=true ایجاد می‌شود
    ///     تمام گزارش‌های قبلی از حالت پیش‌فرض خارج می‌شوند
    /// </summary>
    [Fact(DisplayName = "CreateAsync (Integration) - Unsets other defaults")]
    public async Task CreateAsync_ShouldUnsetOtherDefaults_Integration()
    {
        // ---------- Arrange ----------
        await using var context = DbContextFactory.Create();

        context.ReportDefinitions.AddRange(
            new ReportDefinition
            {
                Id = 1,
                Name = "Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ],
                IsDefault = true
            },
            new ReportDefinition
            {
                Id = 2,
                Name = "2Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ],
                IsDefault = true
            }
        );
        await context.SaveChangesAsync(CancellationToken.None);

        var uow = new UnitOfWork(context);

        var baseTableResolver = new Mock<IBaseTableResolver>();
        baseTableResolver
            .Setup(r => r.Resolve(It.IsAny<List<SelectedColumn>>()))
            .Returns("ResolvedTable");

        var service = new ReportDefinitionService(uow, baseTableResolver.Object);

        var dto = new ReportDefinitionDto
        {
            Name = "Test",
            IsDefault = true,
            SelectedColumns = [new SelectedColumn { Table = "Customers", Column = "FullName" },
                new SelectedColumn { Table = "Customers", Column = "City" },]
        };

        // ---------- Act ----------
        await service.CreateAsync(dto);

        // ---------- Assert ----------
        var reports = context.ReportDefinitions.ToList();

        Assert.Single([reports.FirstOrDefault(r => r.IsDefault)]);
        Assert.Contains(reports, r => r.BaseTable == "ResolvedTable");
    }

    #endregion

    #region UpdateAsync

    /// <summary>
    ///     تست بروزرسانی ستون‌ها و BaseTable وقتی SelectedColumns تغییر کرده است
    /// </summary>
    [Fact(DisplayName = "UpdateAsync - Updates BaseTable when SelectedColumns changed")]
    public async Task UpdateAsync_ShouldUpdateBaseTable_WhenColumnsChanged()
    {
        List<SelectedColumn> list = [];
        var column = new SelectedColumn
        {
            Table = "OldTbl",
            Column = "OldCol"
        };
        list.Add(column);

        var existing = new ReportDefinition
        {
            Id = 1,
            SelectedColumns = list,
            BaseTable = "Orders"
        };

        list = [];
        column = new SelectedColumn
        {
            Table = "NewTbl",
            Column = "NewCol"
        };
        list.Add(column);

        var dto = new ReportDefinitionDto
        {
            SelectedColumns = list,
            IsDefault = false
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _baseTableResolverMock.Setup(b => b.Resolve(dto.SelectedColumns)).Returns("NewTable");

        await _service.UpdateAsync(1, dto);

        Assert.Equal("NewTable", existing.BaseTable);
        _uowMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    ///     تست بروزرسانی زمانی که گزارش با شناسه داده شده وجود ندارد
    /// </summary>
    [Fact(DisplayName = "UpdateAsync - Throws when report not found")]
    public async Task UpdateAsync_ShouldThrow_WhenReportNotFound()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((ReportDefinition?)null);

        var dto = new ReportDefinitionDto();

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _service.UpdateAsync(1, dto)
        );
    }

    #endregion

    #region DeleteAsync

    /// <summary>
    ///     تست حذف گزارش موجود
    /// </summary>
    [Fact(DisplayName = "DeleteAsync - Removes report when exists")]
    public async Task DeleteAsync_ShouldRemoveReport_WhenExists()
    {
        var report = new ReportDefinition
        {
            Id = 1,
            Name = "Customer Orders Report",
            BaseTable = "Customers",
            SelectedColumns =
            [
                new SelectedColumn { Table = "Customers", Column = "FullName" },
                new SelectedColumn { Table = "Customers", Column = "City" },
                new SelectedColumn { Table = "Customers", Column = "Country" },
                new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                new SelectedColumn { Table = "Orders", Column = "Status" },
                new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                new SelectedColumn { Table = "OrderItems", Column = "Total" }
            ]
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

        await _service.DeleteAsync(1);

        List<ReportDefinition> tmpList = [report];
        _repoMock.Verify(r => r.Remove(tmpList), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    ///     تست حذف وقتی گزارش وجود ندارد
    /// </summary>
    [Fact(DisplayName = "DeleteAsync - Throws when report not found")]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ReportDefinition?)null);

        var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _service.DeleteAsync(99));
        Assert.Contains("شناسه وجود ندارد", ex.Message);
    }

    #endregion

    #region SetDefaultAsync

    /// <summary>
    ///     تست تنظیم گزارش به عنوان پیش‌فرض و غیر فعال کردن سایر گزارش‌ها
    /// </summary>
    [Fact(DisplayName = "SetDefaultAsync - Sets only the given report as default")]
    public async Task SetDefaultAsync_ShouldSetOnlyGivenReportAsDefault()
    {
        var reports = new List<ReportDefinition>
        {
            new()
            {
                Id = 1,
                Name = "Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ]
            },
            new()
            {
                Id = 2,
                Name = "Paged Report",
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
            }
        };
        _repoMock.Setup(r => r.GetAllToListAsync()).ReturnsAsync(reports);

        await _service.SetDefaultAsync(2);

        Assert.False(reports[0].IsDefault);
        Assert.True(reports[1].IsDefault);
        _uowMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    /// <summary>
    ///     تست SetDefault وقتی گزارش مورد نظر وجود ندارد
    /// </summary>
    [Fact(DisplayName = "SetDefaultAsync - Throws when report not found")]
    public async Task SetDefaultAsync_ShouldThrow_WhenReportNotFound()
    {
        var reports = new List<ReportDefinition>
        {
            new()
            {
                Id = 1,
                Name = "Customer Orders Report",
                BaseTable = "Customers",
                SelectedColumns =
                [
                    new SelectedColumn { Table = "Customers", Column = "FullName" },
                    new SelectedColumn { Table = "Customers", Column = "City" },
                    new SelectedColumn { Table = "Customers", Column = "Country" },
                    new SelectedColumn { Table = "Orders", Column = "OrderDate" },
                    new SelectedColumn { Table = "Orders", Column = "Status" },
                    new SelectedColumn { Table = "Orders", Column = "TotalAmount" },
                    new SelectedColumn { Table = "OrderItems", Column = "Quantity" },
                    new SelectedColumn { Table = "OrderItems", Column = "Total" }
                ]
            },
            new()
            {
                Id = 2,
                Name = "Paged Report",
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
            }
        };
        _repoMock.Setup(r => r.GetAllToListAsync()).ReturnsAsync(reports);

        var ex = await Assert.ThrowsAsync<NullReferenceException>(() => _service.SetDefaultAsync(99));
        Assert.Contains("شناسه وجود ندارد", ex.Message);
    }

    #endregion
}