namespace DynamicReporting.Api.Infrastructure.Persistence.Query
{
    /// <summary>
    ///     مسئول پیدا کردن کوتاه‌ترین مسیر Join بین دو Entity
    ///     با استفاده از BFS روی گراف FK
    /// </summary>
    public sealed class JoinPathResolver : IJoinPathResolver
    {
        private readonly ConcurrentDictionary<(string From, string To), List<IReadOnlyForeignKey>>
            _joinPathCache = new();

        public List<IReadOnlyForeignKey> Resolve(IEntityType from, IEntityType to)
        {
            return _joinPathCache.GetOrAdd(
                (from.GetTableName()!, to.GetTableName()!),
                _ => FindJoinPath(from, to)
            );
        }

        /// <summary>
        /// پیدا کردن کوتاه‌ترین مسیر Join بین دو EntityType با استفاده از الگوریتم BFS (Breadth-First Search) روی گراف FKها.
        /// - مسیر شامل لیستی از IReadOnlyForeignKey ها است که ترتیب join را تعیین می‌کند.
        /// - BFS تضمین می‌کند که کوتاه‌ترین مسیر از جدول مبدا تا جدول مقصد پیدا شود.
        /// - مسیر می‌تواند شامل FKهای خروجی (Dependent → Principal) و ورودی (Principal ← Dependent) باشد.
        /// - هزینه پردازش: فقط metadata traversal است، مستقل از تعداد رکوردهای جدول.
        /// </summary>
        /// <param name="from">EntityType مبدا</param>
        /// <param name="to">EntityType مقصد</param>
        /// <returns>لیست FKها به ترتیب مسیر Join از جدول مبدا به جدول مقصد</returns>
        /// <exception cref="InvalidOperationException">
        /// پرتاب می‌شود اگر هیچ مسیر ارتباطی بین جدول‌ها وجود نداشته باشد.
        /// </exception>
        private List<IReadOnlyForeignKey> FindJoinPath(IEntityType from, IEntityType to)
        {
            var visited = new HashSet<IEntityType>();
            var queue = new Queue<(IEntityType Entity, List<IReadOnlyForeignKey> Path)>();
            queue.Enqueue((from, []));
            visited.Add(from);

            while (queue.Count > 0)
            {
                var (current, path) = queue.Dequeue();
                if (current == to) return path;

                // بررسی FK خروجی (Dependent → Principal)
                foreach (var fk in current.GetForeignKeys())
                {
                    var next = fk.PrincipalEntityType;
                    if (visited.Add(next))
                    {
                        var newPath = new List<IReadOnlyForeignKey>(path) { fk };
                        queue.Enqueue((next, newPath));
                    }
                }

                // بررسی FK ورودی (Principal ← Dependent)
                foreach (var fk in current.GetReferencingForeignKeys())
                {
                    var next = fk.DeclaringEntityType;
                    if (visited.Add(next))
                    {
                        var newPath = new List<IReadOnlyForeignKey>(path) { fk };
                        queue.Enqueue((next, newPath));
                    }
                }
            }

            throw new InvalidOperationException(
                $"هیچ مسیر ارتباطی (FK) بین جدول‌های '{from.GetTableName()}' و '{to.GetTableName()}' وجود ندارد.");
        }

    }
}
