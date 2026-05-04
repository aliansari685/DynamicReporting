namespace Tests.Infrastructure.Tests.Query;

public class SqlQueryExecutorTests
{
    /// <summary>
    /// اضافه کردن ردیف و در نهایت خروجی گرفتن با کوئری مستقیم به همراه تست یونیت آف ورک
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ExecuteQueryAsync_Should_return_result()
    {
        await using var db = DbContextFactory.Create();

        db.Customers.Add(new Customer { FullName = "Ali Ansari" });
        await db.SaveChangesAsync(CancellationToken.None);

        var executor = new SqlQueryExecutor(db);

        var result = await executor.ExecuteAsync("SELECT FullName FROM Customers", null, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.First().Values.First().Should().Be("Ali Ansari");
    }

}
