namespace DynamicReporting.Api.Domain.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? RegisterDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
