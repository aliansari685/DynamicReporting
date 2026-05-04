namespace DynamicReporting.Api.Domain.Interfaces;

public interface ISqlQueryExecutor
{
    /// <summary>
    ///  متد اجرای کوئری خام
    /// </summary>
    /// <param name="sql">کوئری</param>
    /// <param name="cancellationToken"></param>
    /// <returns>خروجی سلکت</returns>
    Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, CancellationToken cancellationToken = default);


    /// <summary>
    /// اجرای کوئری های اسکالر مشابه کانت
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default);


    /// <summary>
    /// متد اجرای کوئری با پارامتر ها
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// اجرای کوئری های اسکالر مشابه کانت با پارامتر ها
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> ExecuteScalarAsync(string sql, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);
}
