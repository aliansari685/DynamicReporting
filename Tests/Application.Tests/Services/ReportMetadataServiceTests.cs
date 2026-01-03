namespace Tests.Application.Tests.Services;

/// <summary>
/// تست‌های واحد (Unit Tests) مربوط به
/// <see cref="ReportMetadataService"/>.
///
/// این تست‌ها وظیفه دارند صحت عملکرد منطق استخراج
/// و نگاشت متادیتای گزارش را بدون وابستگی به دیتابیس
/// واقعی بررسی نمایند.
/// </summary>
/// <remarks>
/// - وابستگی <see cref="IUnitOfWork"/> به صورت Mock تزریق می‌شود.
/// - این تست‌ها EF Core یا SQL Server واقعی را درگیر نمی‌کنند.
/// - تمرکز اصلی بر صحت Mapping متادیتا و Delegation متدها است.
/// </remarks>
public class ReportMetadataServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReportMetadataService _service;

    public ReportMetadataServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ReportMetadataService(_unitOfWorkMock.Object);
    }

    #region GetAllTableNames

    /// <summary>
    /// بررسی می‌کند که متد GetAllTableNames
    /// لیست نام جداول را مستقیماً از UnitOfWork
    /// دریافت و بازگردانی می‌کند.
    /// </summary>
    [Fact]
    public void GetAllTableNames_Should_Return_TableNames_From_UnitOfWork()
    {
        // Arrange
        var expectedTables = new List<string>
        {
            "dbo.Users",
            "dbo.Orders"
        };

        _unitOfWorkMock
            .Setup(u => u.GetAllFullTableNames())
            .Returns(expectedTables);

        // Act
        var result = _service.GetAllTableNames();

        // Assert
        Assert.Equal(expectedTables, result);
        _unitOfWorkMock.Verify(
            u => u.GetAllFullTableNames(),
            Times.Once);
    }

    #endregion

    #region GetAllMetadata

    /// <summary>
    /// بررسی می‌کند که متد GetAllMetadata
    /// متادیتای جداول را بدون تغییر
    /// از UnitOfWork برمی‌گرداند.
    /// </summary>
    [Fact]
    public void GetAllMetadata_Should_Return_Metadata_From_UnitOfWork()
    {
        // Arrange
        var metadata = new List<TableMetadata>
        {
            new()
            {
                TableName = "Users",
                Columns = []
            },
            new()
            {
                TableName = "Orders",
                Columns = []
            }
        };

        _unitOfWorkMock
            .Setup(u => u.GetAllMetadata())
            .Returns(metadata);

        // Act
        var result = _service.GetAllMetadata();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Users", result[0].TableName);
        Assert.Equal("Orders", result[1].TableName);
    }

    #endregion

    #region GetTableMetadata

    /// <summary>
    /// بررسی می‌کند که متد GetTableMetadata
    /// اطلاعات جدول و ستون‌ها را به درستی
    /// از EntityType استخراج و نگاشت می‌کند.
    /// </summary>
    [Fact]
    public void GetTableMetadata_Should_Return_Correct_Table_And_Columns()
    {
        // Arrange
        var propertyMock = new Mock<IProperty>();
        propertyMock.Setup(p => p.Name).Returns("UserName");
        propertyMock.Setup(p => p.GetColumnName()).Returns("user_name");

        var entityTypeMock = new Mock<IEntityType>();
        entityTypeMock.Setup(e => e.GetTableName()).Returns("Users");
        entityTypeMock.Setup(e => e.ClrType).Returns(typeof(FakeUser));
        entityTypeMock.Setup(e => e.GetProperties())
                      .Returns([propertyMock.Object]);

        _unitOfWorkMock
            .Setup(u => u.GetTrustEntityType("Users"))
            .Returns(entityTypeMock.Object);

        // Act
        var result = _service.GetTableMetadata("Users");

        // Assert
        Assert.Equal("Users", result.TableName);
        Assert.Single(result.Columns);
        Assert.Equal("user_name", result.Columns[0].ColumnName);
        Assert.Equal("نام کاربر", result.Columns[0].Title);
    }

    /// <summary>
    /// بررسی می‌کند که در صورت عدم وجود جدول،
    /// Exception مناسب از UnitOfWork
    /// به لایه بالاتر منتقل می‌شود.
    /// </summary>
    [Fact]
     public void GetTableMetadata_Should_Throw_When_Table_Not_Found()
    {
        // Arrange
        _unitOfWorkMock
            .Setup(u => u.GetTrustEntityType(It.IsAny<string>()))
            .Throws<InvalidOperationException>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _service.GetTableMetadata("InvalidTable"));
    }

    #endregion
}