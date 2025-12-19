namespace DynamicReporting.Api.Domain.Interfaces;

public interface ISqlQueryExecutor
{
    /// <summary>
    /// متد اجرای کوئری
    /// </summary>
    /// <param name="sql">کوئری</param>
    /// <param name="cancellationToken"></param>
    /// <returns>خروجی سلکت</returns>
    Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, CancellationToken cancellationToken = default);
}