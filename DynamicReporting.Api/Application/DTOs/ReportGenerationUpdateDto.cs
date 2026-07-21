namespace DynamicReporting.Api.Application.DTOs;

public class ReportGenerationUpdateDto
{
    [SwaggerSchema("شناسه یکتا گزارش تولید شده")]
    public required Guid ReportGuid { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public int JobId { get; set; }

    [SwaggerSchema("آدرس لینک دانلود فایل گزارش")]
    public string? DownloadUrl { get; set; }

    [SwaggerSchema("زمان حذف یا انقضای اعتبار فایل دانلودی")]
    public DateTime? ExpDateTime { get; set; }
}