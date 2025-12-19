namespace DynamicReporting.Api.Application.Services;

public class ReportDataService(IUnitOfWork unitOfWork, ISqlQueryExecutor sqlExecutor, IReportQueryBuilder queryBuilder) : IReportDataService
{
    private ShopTestDbContext Db => unitOfWork.DbContext;

    public async Task<List<Dictionary<string, object?>>> GetReportDataAsync(int reportDefinitionId)
    {
        var report = await Db.Set<ReportDefinition>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportDefinitionId);

        if (report == null)
            throw new KeyNotFoundException($"گزارش {reportDefinitionId} وجود ندارد");

        var sql = queryBuilder.BuildQuery(report);

        return await sqlExecutor.ExecuteAsync(sql);
    }
}