namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
///     کلاس مرتبط به اجرای کوئری ها با دپر روی دیتابیس
/// </summary>
public class DapperQueryExecutor(ShopTestDbContext dbContext) : ISqlQueryExecutor
{
    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var connection = await GetOpenConnectionAsync(cancellationToken);

        var result =
            await connection.QueryAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        return result.Select(row => ((IDictionary<string, object?>)row)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value,
                StringComparer.OrdinalIgnoreCase)).ToList();
    }

    public async Task<int> ExecuteScalarAsync(string countSql, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var connection = await GetOpenConnectionAsync(cancellationToken);

        var result = await connection.ExecuteScalarAsync(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        return Convert.ToInt32(result);
    }

    /// <summary>
    ///     دریافت connection باز شده از DbContext
    /// </summary>
    private async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        return connection;
    }
}