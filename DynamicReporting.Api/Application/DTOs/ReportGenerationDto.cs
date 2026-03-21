namespace DynamicReporting.Api.Application.DTOs;

public class ReportGenerationDto
{
    [SwaggerSchema("شناسه یکتا گزارش تولید شده")]
    public Guid ReportGuid { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده (ممکن است خالی باشد)")]
    public int? UserId { get; set; }

    [SwaggerSchema("آدرس لینک دانلود فایل گزارش")]
    public string DownloadUrl { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    [SwaggerSchema("زمان حذف یا انقضای اعتبار فایل دانلودی")]
    public DateTime ExpDateTime { get; set; }
}