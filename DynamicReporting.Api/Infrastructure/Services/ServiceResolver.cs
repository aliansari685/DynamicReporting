namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     اجرای الگوی فکتوری
/// </summary>
/// <param name="provider">سرویس پروایدر پروژه</param>
public class ServiceResolver(IServiceProvider provider) : IServiceResolver
{
    public enum ExecutorType
    {
        AdoNet,
        Dapper
    }

    public enum ExportType
    {
        Excel,
        Pdf
    }

    public IExportService GetExportService(ExportType type)
    {
        return provider.GetRequiredKeyedService<IExportService>(type);
    }

    public ISqlQueryExecutor GetExecutorService(ExecutorType type)
    {
        return provider.GetRequiredKeyedService<ISqlQueryExecutor>(type);
    }
}