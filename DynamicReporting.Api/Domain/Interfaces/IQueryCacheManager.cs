namespace DynamicReporting.Api.Domain.Interfaces;

public interface IQueryCacheManager
{
    /// <summary>
    ///     بازگردانی یا ایجاد template query شامل FROM, JOIN و SELECT clauses
    ///     - از cache داخلی با TTL مشخص استفاده می‌کند (مثلاً 3 ساعت)
    ///     - اگر template موجود نباشد، factory فراخوانی شده و نتیجه cache می‌شود
    ///     - هدف کاهش محاسبات تکراری روی query structure و مسیر join است
    /// </summary>
    /// <param name="reportId">شناسه منحصر به فرد گزارش</param>
    /// <param name="factory">Func برای تولید template در صورت نبود cache</param>
    /// <returns>
    ///     Tuple شامل:
    ///     - FromClause: جدول پایه
    ///     - JoinClause: رشته JOIN بین جدول‌ها
    ///     - SelectClause: رشته SELECT بر اساس ستون‌های انتخاب شده
    /// </returns>
    (string FromClause, string JoinClause, string SelectClause) GetOrCreate(int reportId,
        Func<(string, string, string)> factory);
}