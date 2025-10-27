namespace DynamicReporting.Api.Domain.Models;

public class OrderItem
{
    [Key]
    [SwaggerSchema("شناسه آیتم سفارش")]
    public long OrderItemId { get; set; }

    [SwaggerSchema("شناسه سفارش مرتبط")]
    public long? OrderId { get; set; }

    [SwaggerSchema("شناسه محصول")]
    public int? ProductId { get; set; }

    [SwaggerSchema("تعداد محصول")]
    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [SwaggerSchema("قیمت واحد محصول")]
    public decimal? UnitPrice { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    [SwaggerSchema("درصد تخفیف")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    [SwaggerSchema("مبلغ کل آیتم سفارش")]
    public decimal? Total { get; set; }

    [SwaggerSchema("سفارش مربوطه")]
    public virtual Order? Order { get; set; }

    [SwaggerSchema("محصول مربوطه")]
    public virtual Product? Product { get; set; }
}
