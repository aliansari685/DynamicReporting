namespace DynamicReporting.Api.Domain.Models;

public class ReportDefinition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [SwaggerSchema("شناسه یکتا قالب گزارش")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [SwaggerSchema("نام قالب گزارش")]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [SwaggerSchema("جدولی که گزارش بر اساس آن ساخته می‌شود")]
    public string BaseTable { get; set; } = null!;

    [Required]
    [Column(TypeName = "NVARCHAR(MAX)")]
    [SwaggerSchema("ستون‌هایی که کاربر انتخاب کرده به صورت JSON")]
    public string SelectedColumnsJson { get; set; } = null!;

    [Required]
    [Column(TypeName = "NVARCHAR(MAX)")]
    [SwaggerSchema("فیلترهای گزارش به صورت JSON")]
    public string FiltersJson { get; set; } = null!;

    [Required]
    [Column(TypeName = "NVARCHAR(MAX)")]
    [SwaggerSchema("مرتب‌سازی گزارش به صورت JSON")]
    public string SortsJson { get; set; } = null!;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [SwaggerSchema("تاریخ ایجاد قالب گزارش")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SwaggerSchema("تاریخ آخرین بروزرسانی قالب گزارش")]
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    [SwaggerSchema("نام کاربری که گزارش را ایجاد کرده")]
    public string? CreatedBy { get; set; }

    [SwaggerSchema("وضعیت فعال بودن گزارش")]
    public bool IsActive { get; set; } = true;
}