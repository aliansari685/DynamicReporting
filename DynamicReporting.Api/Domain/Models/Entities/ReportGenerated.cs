namespace DynamicReporting.Api.Domain.Models.Entities;

public class ReportGenerated
{
    [Key, Required, SwaggerSchema("شناسه گزارش")]
    public required Guid ReportGuid { get; set; }
    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public int? UserId { get; set; }
    [Required, SwaggerSchema("لینک دانلودفایل")]
    public required string DownloadUrl { get; set; }
    [Required, DataType(DataType.DateTime), SwaggerSchema("زمان حذف و منقضی شدن فایل")]
    public required DateTime DateTime { get; set; }
}