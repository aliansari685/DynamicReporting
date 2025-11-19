namespace DynamicReporting.Api.Domain.Models;

public class ReportDefinition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    // مثال: Orders, Customers, Products
    [Required]
    [MaxLength(100)]
    public string BaseTable { get; set; } = null!;

    // JSON ستون‌هایی که کاربر انتخاب کرده
    [Required]
    public string SelectedColumnsJson { get; set; } = null!;

    // JSON فیلترهای گزارش
    [Required]
    public string FiltersJson { get; set; } = null!;

    // JSON مرتب‌سازی
    [Required]
    public string SortsJson { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? CreatedBy { get; set; }
}