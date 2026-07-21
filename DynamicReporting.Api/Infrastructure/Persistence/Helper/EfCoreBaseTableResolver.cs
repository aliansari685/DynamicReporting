namespace DynamicReporting.Api.Infrastructure.Persistence.Helper;

public sealed class EfCoreBaseTableResolver(ShopTestDbContext dbContext) : IBaseTableResolver
{
    /// <summary>
    ///     child pattern for find base table
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string Resolve(List<SelectedColumn> columns)
    {
        if (columns == null || columns.Count == 0)
            throw new InvalidOperationException("ستونی انتخاب نشده");

        // جدول‌های درگیر در گزارش
        var tables = columns
            .Select(c => c.Table)
            .Distinct()
            .ToList();

        // Rule 1: فقط یک جدول → همون BaseTable
        if (tables.Count == 1)
            return tables[0];

        var entityTypes = dbContext.Model.GetEntityTypes()
            .Where(e => e.GetTableName() != null &&
                        tables.Contains(e.GetTableName()!, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // امتیاز Child بودن (Fact-محور)
        var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);


        // Init همه جدول‌ها (جلوگیری از KeyNotFound)
        foreach (var table in tables) scores[table] = 0;

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
                    scores[tableName] += 3; // Child بودن مهم‌ترین فاکتور
                    scores[principalTable] -= 1; // Parent بودن امتیاز منفی
                }
            }
        }

        // Rule 2: تعداد ستون‌های انتخابی (نیت کاربر)
        foreach (var group in columns.GroupBy(c => c.Table, StringComparer.OrdinalIgnoreCase))
            scores[group.Key] += group.Count() * 2;

        // انتخاب BaseTable نهایی
        return scores
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Key)
            .First()
            .Key;
    }
}