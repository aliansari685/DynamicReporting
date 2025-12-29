namespace Tests.Infrastructure.Tests;

/// <summary>
/// ساخت دیتابیس موقت جهت تست
/// </summary>
public static class DbContextFactory
{
    public static ShopTestDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<ShopTestDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        return new ShopTestDbContext(options);
    }
}