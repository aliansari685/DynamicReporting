namespace DynamicReporting.Api.Application.DTOs;

public class ReportGenerationRequestDto
{
    [Required, SwaggerSchema("شناسه جاب گزارش برای استعلام پروایدر")]
    public required int JobId { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده (ممکن است خالی باشد)")]
    public int? UserId { get; set; }
}