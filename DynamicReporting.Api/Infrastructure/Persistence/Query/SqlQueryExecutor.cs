using DynamicReporting.Api.Infrastructure.Persistence.DbContext;

namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

/// <summary>
/// کلاس مرتبط به اجرای کوئری ها مستقیم روی دیتابیس
/// </summary>
/// <param name="uow"></param>
public sealed class SqlQueryExecutor(IUnitOfWork uow) : ISqlQueryExecutor
{
    private ShopTestDbContext Db => uow.DbContext;

    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(
        string sql,
        CancellationToken cancellationToken = default)
    {
        var result = new List<Dictionary<string, object?>>();

        await using var conn = Db.Database.GetDbConnection();
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
}