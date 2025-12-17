namespace DynamicReporting.Api.Infrastructure.Persistence;

public sealed class EfCoreBaseTableResolver(IUnitOfWork uow) : IBaseTableResolver
{
    public ShopTestDbContext DbContext => uow.DbContext;

    /// <summary>
    /// parent
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// child
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string Resolve1(List<SelectedColumn> columns)
    {
        if (columns == null || columns.Count == 0)
            throw new InvalidOperationException("SelectedColumns cannot be empty");

        // جدول‌های درگیر در گزارش
        var tables = columns
            .Select(c => c.Table)
            .Distinct()
            .ToList();

        // Rule 1: فقط یک جدول → همون BaseTable
        if (tables.Count == 1)
            return tables[0];

        // EntityTypeهای مربوط
        var entityTypes = DbContext.Model.GetEntityTypes()
            .Where(e => tables.Contains(e.GetTableName()!))
            .ToList();

        // امتیاز Child بودن (Fact-محور)
        var scores = new Dictionary<string, int>();

        foreach (var entity in entityTypes)
        {
            var tableName = entity.GetTableName()!;
            scores.TryAdd(tableName, 0);

            // FKهای خروجی = Child بودن
            foreach (var fk in entity.GetForeignKeys())
            {
                var principalTable = fk.PrincipalEntityType.GetTableName();

                if (principalTable != null && tables.Contains(principalTable))
                {
                    scores[tableName] += 3;   // Child بودن مهم‌ترین فاکتور
                    scores[principalTable] -= 1; // Parent بودن امتیاز منفی
                }
            }
        }

        // Rule 2: تعداد ستون انتخابی (Tie-breaker)
        foreach (var group in columns.GroupBy(c => c.Table))
        {
            scores.TryAdd(group.Key, 0);
            scores[group.Key] += group.Count() * 2;
        }

        // انتخاب BaseTable نهایی
        return scores
            .OrderByDescending(x => x.Value)
            .First()
            .Key;
    }

}