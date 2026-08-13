namespace DynamicReporting.Mvc.ViewModels;

/// <summary>
/// DTO for table display information
/// </summary>
public class DisplayTableVm
{
    public string PhysicalName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}