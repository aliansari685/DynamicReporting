namespace DynamicReporting.Api.Domain.Models.Entities;
public class Supplier
{
    [Key]
    [SwaggerSchema("شناسه تأمین‌کننده")]
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(100)]
    [SwaggerSchema("نام تأمین‌کننده")]
    public string SupplierName { get; set; } = null!;

    [MaxLength(50)]
    [SwaggerSchema("شهر تأمین‌کننده")]
    public string? City { get; set; }

    [MaxLength(50)]
    [SwaggerSchema("کشور تأمین‌کننده")]
    public string? Country { get; set; }

    [MaxLength(100)]
    [SwaggerSchema("نام تماس یا مسئول")]
    public string? ContactName { get; set; }

    [MaxLength(30)]
    [SwaggerSchema("شماره تلفن")]
    public string? Phone { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    [SwaggerSchema("ایمیل تأمین‌کننده")]
    public string? Email { get; set; }

    [Column(TypeName = "datetime")]
    [SwaggerSchema("تاریخ ثبت تأمین‌کننده")]
    public DateTime? RegisterDate { get; set; }

    [SwaggerSchema("محصولات مربوط به تأمین‌کننده")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}