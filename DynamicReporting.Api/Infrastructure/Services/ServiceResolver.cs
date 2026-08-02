namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
///     اجرای الگوی فکتوری
/// </summary>
/// <param name="provider">سرویس پروایدر پروژه</param>
public class ServiceResolver(IServiceProvider provider) : IServiceResolver
{
    public IExportService GetExportService(ExportType type)
    {
        return provider.GetKeyedService<IExportService>(type) ??
               throw new InvalidOperationException($"سرویس {type} در حال حاضر وجود ندارد");
    }

    public ISqlQueryExecutor GetExecutorService(ExecutorType type)
    {
        return provider.GetRequiredKeyedService<ISqlQueryExecutor>(type);
    }
}