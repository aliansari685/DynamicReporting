namespace DynamicReporting.Api.Application.DTOs;

public class SupplierDto
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? RegisterDate { get; set; }
}