namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
///     مسئول مدیریت کش template query با TTL
/// </summary>
public sealed class QueryCacheManager(IMemoryCache memoryCache) : IQueryCacheManager
{
    private readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public (string FromClause, string JoinClause, string SelectClause) GetOrCreate(int reportId,
        Func<(string, string, string)> factory)
    {
        return memoryCache.GetOrCreate(reportId, entry =>
        {
            entry.SetOptions(_cacheOptions);
            return factory();
        });
    }
}