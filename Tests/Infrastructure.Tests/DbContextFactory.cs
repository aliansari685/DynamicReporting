namespace Tests.Infrastructure.Tests;

/// <summary>
/// ساخت دیتابیس موقت جهت تست
/// </summary>
public static class DbContextFactory
{
    /// <summary>
    /// کانتکس موقت با استفاده از sql server میسازیم جهت تست توی رم 
    /// </summary>
    /// <param name="dbName"></param>
    /// <returns></returns>
    public static ShopTestDbContext CreateSqlServerContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ShopTestDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        return new ShopTestDbContext(options);
    }

    /// <summary>
    /// کانتکس موقت با استفاده از sqllite میسازیم جهت تست توی رم 
    /// </summary>
    /// <returns></returns>
    public static ShopTestDbContext Create()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShopTestDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ShopTestDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}