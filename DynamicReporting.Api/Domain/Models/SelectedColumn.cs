using System.Diagnostics.CodeAnalysis;

namespace DynamicReporting.Api.Domain.Models;

[NotMapped]
public class SelectedColumn
{
    [SwaggerSchema("نام جدول"), MaxLength(255)]
    public required string Table { get; set; }

    [SwaggerSchema("نام ستون"), MaxLength(255)]
    public required string Column { get; set; }

}