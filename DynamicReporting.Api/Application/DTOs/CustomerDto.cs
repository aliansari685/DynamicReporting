namespace DynamicReporting.Api.Application.DTOs;

public class CustomerDto
{
    [SwaggerSchema("شناسه مشتری")] public long CustomerId { get; set; }

    [SwaggerSchema("نام کامل مشتری")] public string FullName { get; set; } = string.Empty;

    [SwaggerSchema("ایمیل مشتری")] public string Email { get; set; } = string.Empty;

    [SwaggerSchema("شماره تلفن مشتری")] public string Phone { get; set; } = string.Empty;

    [SwaggerSchema("شهر مشتری")] public string City { get; set; } = string.Empty;

    [SwaggerSchema("کشور مشتری")] public string Country { get; set; } = string.Empty;

    [SwaggerSchema("تاریخ ثبت نام مشتری")] public DateTime? RegisterDate { get; set; }

    [SwaggerSchema("وضعیت فعال بودن مشتری")]
    public bool IsActive { get; set; } = true;
}