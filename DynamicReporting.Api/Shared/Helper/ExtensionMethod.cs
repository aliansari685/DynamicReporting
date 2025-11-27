namespace DynamicReporting.Api.Shared.Helper;

public class ExtensionMethod
{
    public static List<string> GetPropertyNames<T>()
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();
    }

    public static List<string> GetPropertyNames(Type modelType)
    {
        return modelType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToList();
    }
}