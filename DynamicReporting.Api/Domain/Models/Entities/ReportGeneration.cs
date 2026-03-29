namespace DynamicReporting.Api.Domain.Models.Entities;

public class ReportGeneration
{
    [Key, Required, SwaggerSchema(" شناسه گزارش برای درخواست و پاسخ")]
    public /*required*/ Guid ReportGuid { get; set; } //= new guid();
    [Required, SwaggerSchema("شناسه جاب گزارش برای استعلام پروایدر")]
    public required int JobId { get; set; }
    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public int? UserId { get; set; }
    [Required, MaxLength(2048), SwaggerSchema("لینک دانلودفایل")]
    public string? DownloadUrl { get; set; }
    [Required, DataType(DataType.DateTime), SwaggerSchema("زمان حذف و منقضی شدن فایل")]
    public DateTime ExpDateTime { get; set; }
}