namespace DynamicReporting.Api.Infrastructure.Persistence.Query
{
    /// <summary>
    ///     مسئول ساخت SELECT clause و JOIN string
    /// </summary>
    public sealed class SelectJoinBuilder(IJoinPathResolver pathResolver) : ISelectJoinBuilder
    {
        public string BuildSelectClause(IEnumerable<SelectedColumn> columns) =>
            string.Join(", ",
                columns.Select(c =>
                    $"[{c.Table}].[{c.Column}] AS [{c.Table}_{c.Column}]"));

        public string BuildJoinClause(string baseTable, List<SelectedColumn> columns, Func<string, IEntityType> getEntityType)
        {
            //todo : trace code to find table list for joins
            var joinedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { baseTable };
            var joins = new StringBuilder();

            var targetTables = columns
                .Select(c => c.Table)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(t => !t.Equals(baseTable, StringComparison.OrdinalIgnoreCase));

            foreach (var targetTable in targetTables)
            {
                if (joinedTables.Contains(targetTable)) continue;

                var fromEntity = getEntityType(baseTable);
                var toEntity = getEntityType(targetTable);

                var joinPath = pathResolver.Resolve(fromEntity, toEntity);
                foreach (var fk in joinPath) AppendJoinIfNeeded(fk, joinedTables, joins);
            }

            return joins.ToString();
        }

        /// <summary>
        /// اضافه کردن یک JOIN به query در صورتی که جدول مقصد هنوز به query اضافه نشده باشد.
        /// - بررسی می‌کند که یکی از جدول‌های Principal یا Dependent قبلاً join شده باشد.
        /// - اگر هیچکدام join نشده باشند، خطا پرتاب می‌شود (ترتیب مسیر Join نامعتبر است).
        /// - تعیین می‌کند که کدام جدول از سمت FROM و کدام جدول از سمت TO باشد
        ///   بر اساس اینکه جدول‌ها قبلاً join شده‌اند.
        /// - LEFT JOIN ایجاد می‌کند و به StringBuilder اضافه می‌شود.
        /// - جدول مقصد بعد از اضافه شدن به joinedTables ثبت می‌شود تا از اضافه شدن دوباره جلوگیری شود.
        /// </summary>
        /// <param name="fk">ForeignKey بین دو Entity که مسیر Join را مشخص می‌کند</param>
        /// <param name="joinedTables">مجموعه جدول‌هایی که تا این لحظه به query اضافه شده‌اند</param>
        /// <param name="joins">StringBuilder که رشته JOIN نهایی را ذخیره می‌کند</param>
        /// <exception cref="InvalidOperationException">
        /// پرتاب می‌شود اگر هیچ یک از جدول‌های Principal یا Dependent قبلاً join نشده باشند
        /// (مسیر Join نامعتبر است)
        /// </exception>
        private void AppendJoinIfNeeded(
            IReadOnlyForeignKey fk,
            HashSet<string> joinedTables,
            StringBuilder joins)
        {
            var principalTable = fk.PrincipalEntityType.GetTableName()!;
            var dependentTable = fk.DeclaringEntityType.GetTableName()!;
            var principalKey = fk.PrincipalKey.Properties[0].GetColumnName();
            var foreignKey = fk.Properties[0].GetColumnName();

            var dependentJoined = joinedTables.Contains(dependentTable);
            var principalJoined = joinedTables.Contains(principalTable);

            if (!dependentJoined && !principalJoined)
                throw new InvalidOperationException("ترتیب مسیر Join نامعتبر است و امکان ساخت Join وجود ندارد.");

            // تعیین جهت Join
            var fromTable = dependentJoined ? dependentTable : principalTable;
            var fromColumn = dependentJoined ? foreignKey : principalKey;
            var toTable = dependentJoined ? principalTable : dependentTable;
            var toColumn = dependentJoined ? principalKey : foreignKey;

            // اضافه کردن جدول مقصد به joinedTables و ساخت رشته JOIN
            if (joinedTables.Add(toTable))
                joins.AppendLine(
                    $"LEFT JOIN [{toTable}] ON [{toTable}].[{toColumn}] = [{fromTable}].[{fromColumn}]");
        }
    }
}