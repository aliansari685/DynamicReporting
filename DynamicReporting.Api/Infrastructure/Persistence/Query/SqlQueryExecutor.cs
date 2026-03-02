namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
/// کلاس مرتبط به اجرای کوئری ها مستقیم روی دیتابیس
/// </summary>
public class SqlQueryExecutor(ShopTestDbContext dbContext) : ISqlQueryExecutor
{
    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, CancellationToken cancellationToken = default)
    {
        var result = new List<Dictionary<string, object?>>();

        await using var cmd = await CreateSqlCommandConnectionAsync(sql, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        var columnNames = Enumerable
            .Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .ToArray();

        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var row = new Dictionary<string, object?>(
                columnNames.Length,
                StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < columnNames.Length; i++)
                row[columnNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);

            result.Add(row);
        }
        return result;
    }

    public async Task<int> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default)
    {
        await using var cmd = await CreateSqlCommandConnectionAsync(sql, cancellationToken);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private async Task<DbCommand> CreateSqlCommandConnectionAsync(string sql, CancellationToken cancellationToken)
    {
        DbCommand? cmd = null;
        try
        {
            var conn = dbContext.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }
        catch
        {
            if (cmd != null) await cmd.DisposeAsync();
            throw;
        }
    }
}