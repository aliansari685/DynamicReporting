namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
/// کلاس مرتبط به اجرای کوئری ها مستقیم روی دیتابیس
/// </summary>
public sealed class SqlQueryExecutor(ShopTestDbContext dbContext) : ISqlQueryExecutor
{
    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql, CancellationToken cancellationToken = default)
    {
        var result = new List<Dictionary<string, object?>>();

        //todo: error : System.InvalidOperationException: 'The ConnectionString property has not been initialized

        await using var conn = dbContext.Database.GetDbConnection();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(cancellationToken);

        await using var reader = await cmd.ExecuteReaderAsync(
            CommandBehavior.SequentialAccess,
            cancellationToken);

        var columnNames = Enumerable
            .Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .ToArray();

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(
                columnNames.Length,
                StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < columnNames.Length; i++)
            {
                row[columnNames[i]] =
                    reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            result.Add(row);
        }

        return result;
    }

    public async Task<int> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default)
    {
        await using var conn = dbContext.Database.GetDbConnection();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(cancellationToken);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(result);
    }
}