namespace Tests.Infrastructure.Tests.Helper;

public class EfCoreBaseTableResolverTests
{
    /// <summary>
    /// وقتی هیچ ستونی انتخاب نشده باشد باید خطا برگردد
    /// </summary>
    [Fact(DisplayName = "Resolve - Throws when no columns provided")]
    public void Resolve_ShouldThrow_WhenColumnsEmpty()
    {
        using var context = DbContextFactory.Create();
        var resolver = new EfCoreBaseTableResolver(context);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            resolver.Resolve([]));

        Assert.Contains("ستونی انتخاب نشده", ex.Message);
    }

    /// <summary>
    /// اگر فقط یک جدول درگیر باشد همان جدول BaseTable است
    /// </summary>
    [Fact(DisplayName = "Resolve - Returns table when only one table exists")]
    public void Resolve_ShouldReturnSameTable_WhenSingleTable()
    {
        using var context = DbContextFactory.Create();
        var resolver = new EfCoreBaseTableResolver(context);

        var columns = new List<SelectedColumn>
        {
            new() { Table = "Orders", Column = "Id" },
            new() { Table = "Orders", Column = "CustomerId" }
        };

        var baseTable = resolver.Resolve(columns);

        Assert.Equal("Orders", baseTable);
    }

    /// <summary>
    /// جدول Child (دارای FK خروجی) باید به عنوان BaseTable انتخاب شود
    /// </summary>
    [Fact(DisplayName = "Resolve - Chooses child table based on foreign keys")]
    public void Resolve_ShouldPreferChildTable()
    {
        using var context = DbContextFactory.Create();
        var resolver = new EfCoreBaseTableResolver(context);

        var columns = new List<SelectedColumn>
        {
            new() { Table = "Customers", Column = "Id" },
            new() { Table = "Orders", Column = "Id" }
        };

        var baseTable = resolver.Resolve(columns);

        // Orders → FK به Customers دارد → Child
        Assert.Equal("Orders", baseTable);
    }

    /// <summary>
    /// اگر نیت کاربر روی یک جدول بیشتر باشد، باید در انتخاب BaseTable اثر بگذارد
    /// </summary>
    [Fact(DisplayName = "Resolve - Considers user intent via selected columns count")]
    public void Resolve_ShouldConsiderUserIntent()
    {
        using var context = DbContextFactory.Create();
        var resolver = new EfCoreBaseTableResolver(context);

        var columns = new List<SelectedColumn>
        {
            new() { Table = "Orders", Column = "Id" },
            new() { Table = "Orders", Column = "CustomerId" },
            new() { Table = "Orders", Column = "CreatedAt" },

            new() { Table = "Customers", Column = "Name" }
        };

        var baseTable = resolver.Resolve(columns);

        Assert.Equal("Orders", baseTable);
    }
}
