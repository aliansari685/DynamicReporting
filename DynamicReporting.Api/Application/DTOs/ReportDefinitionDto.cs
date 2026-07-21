namespace DynamicReporting.Api.Application.DTOs;

public class ReportDefinitionDto
{
    [SwaggerSchema("نام قالب گزارش")] public string Name { get; set; } = string.Empty;

    [SwaggerSchema("ستون‌های انتخاب شده برای گزارش")]
    public List<SelectedColumn> SelectedColumns { get; set; } = [];

    [SwaggerSchema("کاربری که گزارش را ایجاد کرده است")]
    public string CreatedBy { get; set; } = string.Empty;

    [SwaggerSchema("آیا این قالب به‌صورت پیش‌فرض است؟")]
    public bool IsDefault { get; set; }

    [SwaggerSchema("جدولی که گزارش بر اساس آن ساخته می‌شود")]
    public string BaseTable { get; set; } = string.Empty;
}