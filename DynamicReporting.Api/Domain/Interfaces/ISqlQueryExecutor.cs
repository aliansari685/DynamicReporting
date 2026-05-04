namespace DynamicReporting.Api.Domain.Interfaces;

public interface ISqlQueryExecutor
{
    /// <summary>
    /// متد اجرای کوئری با پارامتر ها
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// اجرای کوئری های اسکالر مشابه کانت با پارامتر ها
    /// </summary>
    /// <param name="countSql"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> ExecuteScalarAsync(string countSql, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

}
