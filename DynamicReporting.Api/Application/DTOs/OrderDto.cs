namespace DynamicReporting.Api.Application.DTOs
{
    public class OrderDto
    {
        public long OrderId { get; set; }

        public long? CustomerId { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? Status { get; set; }

        public string? PaymentType { get; set; }

        public string? ShippingCity { get; set; }

        public string? ShippingCountry { get; set; }
    }
}
