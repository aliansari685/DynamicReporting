namespace DynamicReporting.Mvc.ViewModels;

public sealed class FilterConditionVm
{
    public string Field { get; init; } = string.Empty;
    public string Operator { get; init; } = string.Empty;
    public object? Value { get; init; }
}