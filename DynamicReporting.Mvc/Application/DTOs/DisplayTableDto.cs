namespace DynamicReporting.Mvc.Application.Services;

/// <summary>
/// DTO for table display information
/// </summary>
public class DisplayTableDto
{
    public string PhysicalName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}