namespace DynamicReporting.Api.Infrastructure.Services;

/// <summary>
/// اجرای الگوی فکتوری 
/// </summary>
/// <param name="provider">سرویس پروایدر پروژه</param>
public class ServiceResolver(IServiceProvider provider) : IServiceResolver
{
    public enum ExportType
    {
        Excel, Pdf
    }
    public enum ExecutorType
    {
        AdoNet, Dapper
    }
    public IReportExportService GetExportService(ExportType type)
    {
        return provider.GetRequiredKeyedService<IReportExportService>(type);
    }

    public ISqlQueryExecutor GetExecutorService(ExecutorType type)
    {
        return provider.GetRequiredKeyedService<SqlQueryExecutor>(type);
    }
}