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


    /// <summary>
    /// اجرای کوئری ها مشابه کانت
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default);
}