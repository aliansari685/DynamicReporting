namespace DynamicReporting.Api.Domain.Models.Entities;

[SwaggerSchema("سفارشات")]
public class Order
{
    [Key, SwaggerSchema("شناسه سفارش")]
    public long OrderId { get; set; }

    [SwaggerSchema("شناسه مشتری")]
    public long? CustomerId { get; set; }

    [Column(TypeName = "datetime"), SwaggerSchema("تاریخ ثبت سفارش")]
    public DateTime? OrderDate { get; set; }

    [Column(TypeName = "decimal(12, 2)"), SwaggerSchema("مبلغ کل سفارش")]
    public decimal? TotalAmount { get; set; }

    [MaxLength(20), SwaggerSchema("وضعیت سفارش")]
    public string? Status { get; set; }

    [MaxLength(20), SwaggerSchema("نوع پرداخت")]
    public string? PaymentType { get; set; }

    [MaxLength(50), SwaggerSchema("شهر مقصد ارسال")]
    public string? ShippingCity { get; set; }

    [MaxLength(50), SwaggerSchema("کشور مقصد ارسال")]
    public string? ShippingCountry { get; set; }

    [SwaggerSchema("مشتری سفارش")]
    public virtual Customer? Customer { get; set; }

    [SwaggerSchema("آیتم‌های سفارش")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = (List<OrderItem>)[];
}
