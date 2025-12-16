namespace DynamicReporting.Api.Infrastructure.Persistence;

/// <summary>
/// این کلاس جدول پایه را برای کوئری ها پیدا میکند با توجه ب فارن کی و جوین ها
/// </summary>
/// <param name="uow"></param>
public sealed class EfCoreBaseTableResolver(IUnitOfWork uow) : IBaseTableResolver
{
    public ShopTestDbContext DbContext => uow.DbContext;
    public string Resolve(List<SelectedColumn> columns)
    {
        if (columns == null || columns.Count == 0)
            throw new InvalidOperationException("SelectedColumns cannot be empty");

        var tables = columns
            .Select(c => c.Table)
            .Distinct()
            .ToList();

        // Rule 1 & 2: یک جدول
        if (tables.Count == 1)
            return tables[0];

        // Rule 3: تحلیل FK
        //پیدا کردن روابط
        var entityTypes = DbContext.Model.GetEntityTypes()
            .Where(e => tables.Contains(e.GetTableName()!))
            .ToList();

        var parentScores = new Dictionary<string, int>();

        foreach (var entity in entityTypes)
        {
            foreach (var fk in entity.GetForeignKeys())
            {
                var principalTable = fk.PrincipalEntityType.GetTableName();

                if (principalTable != null && tables.Contains(principalTable))
                {
                    parentScores.TryAdd(principalTable, 0);
                    parentScores[principalTable]++;
                }
            }
        }

        return parentScores.Any()
            ? parentScores
                .OrderByDescending(x => x.Value)
                .First()
                .Key
            :
            columns
                .GroupBy(c => c.Table)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
    }
}