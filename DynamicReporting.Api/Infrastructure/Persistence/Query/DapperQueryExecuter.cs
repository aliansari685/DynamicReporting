//using Microsoft.EntityFrameworkCore;

//namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

//public class DapperQueryExecuter : ISqlQueryExecutor
//{
//    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(
//        string sql,
//        CancellationToken cancellationToken = default)
//    {
//        return await ExecuteAsync(sql, null, cancellationToken);
//    }

//    public async Task<int> ExecuteScalarAsync(
//        string sql,
//        CancellationToken cancellationToken = default)
//    {
//        return await ExecuteScalarAsync(sql, null, cancellationToken);
//    }

//    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(
//        string sql,
//        Dictionary<string, object>? parameters = null,
//        CancellationToken cancellationToken = default)
//    {
//        var connection = await GetOpenConnectionAsync(cancellationToken);

//        // اجرای کوئری با Dapper و بازگشت به صورت IEnumerable<dynamic>
//        var result = await connection.QueryAsync(
//            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

//        // تبدیل به List<Dictionary<string, object?>>
//        return result
//            .Select(row => ((IDictionary<string, object>)row)
//                .ToDictionary(
//                    kvp => kvp.Key,
//                    kvp => kvp.Value,
//                    StringComparer.OrdinalIgnoreCase))
//            .ToList();
//    }

//    public async Task<int> ExecuteScalarAsync(
//        string sql,
//        Dictionary<string, object>? parameters = null,
//        CancellationToken cancellationToken = default)
//    {
//        var connection = await GetOpenConnectionAsync(cancellationToken);

//        var result = await connection.ExecuteScalarAsync(
//            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

//        return Convert.ToInt32(result);
//    }

//    /// <summary>
//    /// دریافت connection باز شده از DbContext
//    /// </summary>
//    private async Task<System.Data.Common.DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken)
//    {
//        var connection = dbContext.Database.GetDbConnection();

//        if (connection.State != System.Data.ConnectionState.Open)
//            await connection.OpenAsync(cancellationToken);

//        return connection;
//    }
//}
//}