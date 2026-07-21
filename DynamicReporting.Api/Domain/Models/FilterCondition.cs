namespace DynamicReporting.Api.Domain.Models;

/// <summary>
///     مدل مربوط به فیلتر ارسالی از کلاینت
/// </summary>
public class FilterCondition
{
    [SwaggerSchema("نام ستون")]
    [MaxLength(255)]
    public string Field { get; set; } = "";

    [SwaggerSchema("نام عملیات")]
    [MaxLength(255)]
    public string Operator { get; set; } = "";

    [SwaggerSchema("مقدار ")]
    [MaxLength(255)]
    public object Value { get; set; } = "";
}