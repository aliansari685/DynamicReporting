namespace DynamicReporting.Api.Application.DTOs;

public class ReportGenerationResponseDto
{
    [SwaggerSchema("شناسه یکتا گزارش تولید شده")]
    public Guid ReportGuid { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public required int JobId { get; set; }

    [SwaggerSchema("وضعیت گزارش")] public required string Status { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده (ممکن است خالی باشد)")]
    public int? UserId { get; set; }

    [SwaggerSchema("آدرس لینک دانلود فایل گزارش")]
    public string? DownloadUrl { get; set; }

    [SwaggerSchema("زمان حذف یا انقضای اعتبار فایل دانلودی")]
    public DateTime ExpDateTime { get; set; }

    [SwaggerSchema("زمان ایجاد ردیف ")] public DateTime CreateAt { get; set; }

    [SwaggerSchema("پسوند فایل")] public string? FileType { get; set; }
    [SwaggerSchema("ایدی قالب گزارش")] public required int ReportDefinitionId { get; set; }
}