namespace DynamicReporting.Api.Domain.Models;

public sealed class PagedResult<T>
{
    [SwaggerSchema("ردیف ها")]
    public IReadOnlyList<T> Data { get; init; } = [];

    [SwaggerSchema("تعداد کل")]
    public int TotalCount { get; init; }

    [SwaggerSchema("صفحه جاری")]
    public int Page { get; init; }

    [SwaggerSchema("تعداد هر ردیف در صفحه")]
    public int Take { get; init; }

    [SwaggerSchema("مجموع صفحات")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Take);
}