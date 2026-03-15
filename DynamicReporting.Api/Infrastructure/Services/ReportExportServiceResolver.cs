namespace DynamicReporting.Api.Infrastructure.Services;

public class ReportExportServiceResolver(IKeyedServiceProvider provider) : IReportExportServiceResolver
{
    public IReportExportService GetService(string type)
    {
        return provider.GetRequiredKeyedService<IReportExportService>(type);
    }
}