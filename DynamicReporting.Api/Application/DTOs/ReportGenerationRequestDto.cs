namespace DynamicReporting.Api.Application.DTOs;

public class ReportGenerationRequestDto
{
    [SwaggerSchema(" شناسه گزارش برای درخواست و پاسخ")]
    public required Guid ReportGuid { get; set; }
    [SwaggerSchema("شناسه جاب گزارش برای استعلام پروایدر")]
    public required int JobId { get; set; }
    [SwaggerSchema("شناسه کاربر درخواست دهنده (ممکن است خالی باشد)")]
    public int? UserId { get; set; }
}