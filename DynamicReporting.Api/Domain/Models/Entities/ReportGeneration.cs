namespace DynamicReporting.Api.Domain.Models.Entities;

public class ReportGeneration
{
    [Key, Required, SwaggerSchema(" شناسه گزارش برای درخواست و پاسخ")]
    public required Guid ReportGuid { get; set; }
    [Required, SwaggerSchema("شناسه جاب گزارش برای استعلام پروایدر")]
    public int JobId { get; set; }
    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public int? UserId { get; set; }
    [Required, MaxLength(2048), SwaggerSchema("لینک دانلودفایل")]
    public required string DownloadUrl { get; set; }
    [Required, DataType(DataType.DateTime), SwaggerSchema("زمان حذف و منقضی شدن فایل")]
    public required DateTime ExpDateTime { get; set; } = DateTime.UtcNow.AddHours(3);
}