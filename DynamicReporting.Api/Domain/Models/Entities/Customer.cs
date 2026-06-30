namespace DynamicReporting.Api.Domain.Models.Entities
{
    [SwaggerSchema("مشتریان")]
    public class Customer
    {
        [Key, SwaggerSchema("شناسه مشتری")]
        public long CustomerId { get; set; }

        [Required, MaxLength(100), SwaggerSchema("نام کامل مشتری")]
        public required string FullName { get; set; }

        [EmailAddress, MaxLength(100), SwaggerSchema("ایمیل مشتری")]
        public string? Email { get; set; }

        [Phone, MaxLength(30), SwaggerSchema("شماره تلفن مشتری")]
        public string? Phone { get; set; }

        [MaxLength(50), SwaggerSchema("شهر مشتری")]
        public string? City { get; set; }

        [MaxLength(50), SwaggerSchema("کشور مشتری")]
        public string? Country { get; set; }

        [Column(TypeName = "datetime"), SwaggerSchema("تاریخ ثبت نام مشتری")]
        public DateTime? RegisterDate { get; set; }

        [SwaggerSchema("وضعیت فعال بودن مشتری")]
        public bool IsActive { get; set; } = true;

        [SwaggerSchema("سفارشات مشتری")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}