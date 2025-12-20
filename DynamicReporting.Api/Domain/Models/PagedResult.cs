namespace DynamicReporting.Api.Domain.Models;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public int TotalCount { get; init; }

    public int Page { get; init; }
    public int PageSize { get; init; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);
}