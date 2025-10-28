namespace DynamicReporting.Api.Application.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsAvailable { get; set; }
}