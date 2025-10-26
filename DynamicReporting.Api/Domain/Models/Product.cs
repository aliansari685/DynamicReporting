using System;
using System.Collections.Generic;

namespace DynamicReporting.Api.Domain.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier? Supplier { get; set; }
}
