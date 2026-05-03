using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DynamicReporting.Api.Infrastructure.Persistence.Context;

/// <summary>
/// ایجاد کانتکسم با روش دیتابیس فرست
/// تنظیمات بطوری انجام شده ک بتوان مایگریشن بعدا انجام داد
/// </summary>
public class ShopTestDbContext(DbContextOptions<ShopTestDbContext> options) : DbContext(options)
{
    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }

    public virtual DbSet<ReportGeneration> ReportGenerations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //var selectedColumnConverter = new ValueConverter<List<SelectedColumn>, string>(
        //    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
        //    v => JsonSerializer.Deserialize<List<SelectedColumn>>
        //        (v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);


        var serializerOptions = new JsonSerializerOptions();

        var selectedColumnConverter = new ValueConverter<List<SelectedColumn>, string>(
            v => JsonSerializer.Serialize(v, serializerOptions),
            v => JsonSerializer.Deserialize<List<SelectedColumn>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SelectedColumn>()
        );

        var selectedColumnComparer = new ValueComparer<List<SelectedColumn>>(
            (c1, c2) =>
                (c1 == null && c2 == null) ||
                (c1 != null && c2 != null &&
                 JsonSerializer.Serialize(c1, serializerOptions) ==
                 JsonSerializer.Serialize(c2, serializerOptions)),
            c => JsonSerializer.Serialize(c, serializerOptions).GetHashCode(),
            c => c.ToList()
        );

        modelBuilder.Entity<ReportDefinition>()
            .Property(r => r.SelectedColumns)
            .HasConversion(selectedColumnConverter)
            .Metadata.SetValueComparer(selectedColumnComparer);

        modelBuilder.Entity<ReportGeneration>()
            .HasIndex(e => e.JobId, "IX_JobId")
            .IsUnique();
    }
}