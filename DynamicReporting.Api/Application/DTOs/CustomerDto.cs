namespace DynamicReporting.Api.Application.DTOs
{
    public class CustomerDto
    {
        [SwaggerSchema("شناسه مشتری")]
        public long CustomerId { get; set; }

        [SwaggerSchema("نام کامل مشتری")]
        public string FullName { get; set; } = null!;

        [SwaggerSchema("ایمیل مشتری")]
        public string? Email { get; set; }

        [SwaggerSchema("شماره تلفن مشتری")]
        public string? Phone { get; set; }

        [SwaggerSchema("شهر مشتری")]
        public string? City { get; set; }

        [SwaggerSchema("کشور مشتری")]
        public string? Country { get; set; }

        [SwaggerSchema("تاریخ ثبت نام مشتری")]
        public DateTime? RegisterDate { get; set; }

        [SwaggerSchema("وضعیت فعال بودن مشتری")]
        public bool IsActive { get; set; } = true;
    }
}