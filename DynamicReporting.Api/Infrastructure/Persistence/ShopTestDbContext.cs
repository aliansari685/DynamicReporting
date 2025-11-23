using System.Text.Json;

namespace DynamicReporting.Api.Infrastructure.Persistence;

/// <summary>
/// ایجاد کانتکسم با روش دیتابیس فرست
/// تنظیمات بطوری انجام شده ک بتوان مایگریشن بعدا انجام داد
/// </summary>
public class ShopTestDbContext : DbContext
{
    public ShopTestDbContext()
    {
    }

    public ShopTestDbContext(DbContextOptions<ShopTestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        modelBuilder.Entity<ReportDefinition>()
            .Property(r => r.SelectedColumns)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<SelectedColumn>>(v, jsonOptions)
            );
    }
}
