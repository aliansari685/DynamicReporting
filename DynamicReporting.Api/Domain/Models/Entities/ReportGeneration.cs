namespace DynamicReporting.Api.Domain.Models.Entities;

[SwaggerSchema("گزارش های ساخته شده")]
public class ReportGeneration
{
    [Key]
    [Required]
    [SwaggerSchema(" شناسه گزارش برای درخواست و پاسخ")]
    public Guid ReportGuid { get; set; }

    [Required]
    [SwaggerSchema("شناسه جاب گزارش برای استعلام پروایدر")]
    public int JobId { get; set; }

    [SwaggerSchema("شناسه کاربر درخواست دهنده")]
    public int? UserId { get; set; }

    [MaxLength(2048)]
    [SwaggerSchema("لینک دانلودفایل")]
    public string? DownloadUrl { get; set; }

    [DataType(DataType.Date)]
    [SwaggerSchema("زمان حذف و منقضی شدن فایل")]
    public DateTime? ExpDateTime { get; set; }

    [DataType(DataType.Date)]
    [SwaggerSchema("زمان ایجاد ردیف")]
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    [MaxLength(10)]
    [SwaggerSchema("پسوند فایل")]
    public string? FileType { get; set; }
    //test for commit
}