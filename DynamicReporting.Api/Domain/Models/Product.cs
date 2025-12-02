using System.Text.Json.Serialization;

namespace DynamicReporting.Api.Domain.Models;

public class Product
{
    [Key]
    [SwaggerSchema("شناسه محصول")]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    [SwaggerSchema("نام محصول")]
    public string ProductName { get; set; } = null!;

    [MaxLength(50)]
    [SwaggerSchema("دسته‌بندی محصول")]
    public string? Category { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [SwaggerSchema("قیمت محصول")]
    public decimal? Price { get; set; }

    [SwaggerSchema("موجودی انبار")]
    public int? Stock { get; set; }

    [SwaggerSchema("شناسه تأمین‌کننده")]
    public int? SupplierId { get; set; }

    [Column(TypeName = "datetime")]
    [SwaggerSchema("تاریخ ایجاد محصول")]
    public DateTime? CreatedDate { get; set; }

    [SwaggerSchema("وضعیت در دسترس بودن محصول")]
    public bool? IsAvailable { get; set; }

    [SwaggerSchema("آیتم‌های سفارش مرتبط با محصول")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [SwaggerSchema("تأمین‌کننده محصول")]
    public virtual Supplier? Supplier { get; set; }
}