using DynamicReporting.Api.Domain.Enums;

namespace DynamicReporting.Api.Domain.Models;

public class SortableColumnDto
{
    /// <summary>
    ///     نام ستون
    /// </summary>
    public string? Column { get; set; }

    /// <summary>
    ///     مرتب سازی صعودی و نزولی
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}