namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
///     کلاس مرتبط به اجرای کوئری ها مستقیم روی دیتابیس
/// </summary>
public class SqlQueryExecutor(ShopTestDbContext dbContext) : ISqlQueryExecutor
{
    public async Task<int> ExecuteScalarAsync(string countSql, Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var cmd = await CreateSqlCommandConnectionAsync(countSql, parameters, cancellationToken);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(string sql,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var result = new List<Dictionary<string, object?>>();

        await using var cmd = await CreateSqlCommandConnectionAsync(sql, parameters, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        var physicalNames = Enumerable
            .Range(0, reader.FieldCount)
            .Select(reader.GetName)
            .ToArray();

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(physicalNames.Length, StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < physicalNames.Length; i++)
                row[physicalNames[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);

            result.Add(row);
        }

        return result;
    }

    private async Task<DbCommand> CreateSqlCommandConnectionAsync(string sql,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        DbCommand? cmd = null;
        try
        {
            var conn = dbContext.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            // اضافه کردن پارامترها به Command
            if (parameters != null)
                foreach (var param in parameters)
                {
                    var dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value;

                    if (param.Value is string stringValue && stringValue.Contains('%'))
                        dbParameter.DbType = DbType.String;

                    cmd.Parameters.Add(dbParameter);
                }

            return cmd;
        }
        catch
        {
            if (cmd != null) await cmd.DisposeAsync();
            throw;
        }
    }
}