using DynamicReporting.Api.Domain.Models.Entities;
using DynamicReporting.Api.Infrastructure.Persistence.Context;
using DynamicReporting.Api.Infrastructure.Persistence.Query;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Tests.Query;

public class SqlQueryExecutorTests
{
    private ShopTestDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ShopTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ShopTestDbContext(options);
    }

    [Fact]
    public async Task ExecuteQueryAsync_Should_return_result()
    {
        await using var db = CreateDb();
        db.Customers.Add(new Customer { FullName = "Ali Ansari" });
        await db.SaveChangesAsync();

        var executor = new SqlQueryExecutor(db);

        var result = await executor.ExecuteAsync(
            "SELECT Name FROM Customers");

        result.Should().NotBeEmpty();
        result.First().Values.First().Should().Be("Ali");
    }
}
