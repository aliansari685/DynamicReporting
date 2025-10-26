using System;
using System.Collections.Generic;

namespace DynamicReporting.Api.Domain.Models;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public DateTime? RegisterDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
