namespace DynamicReporting.Api.Application.DTOs;

public class OrderItemDto
{
    public long OrderItemId { get; set; }

    public long? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Total { get; set; }
}