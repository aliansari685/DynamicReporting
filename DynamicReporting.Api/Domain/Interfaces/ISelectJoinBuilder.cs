namespace DynamicReporting.Api.Domain.Interfaces;

public interface ISelectJoinBuilder
{
    /// <summary>
    ///     تولید بخش SELECT query بر اساس ستون‌های انتخاب شده
    ///     - نام ستون‌ها به صورت Table_Column alias می‌شوند تا ambiguity در join برطرف شود
    ///     - کاملاً رشته‌ای و lightweight است، هزینه‌ی CPU بسیار کم
    /// </summary>
    /// <param name="columns">ستون‌های انتخاب شده برای گزارش</param>
    /// <returns>رشته SQL بخش SELECT</returns>
    string BuildSelectClause(IEnumerable<SelectedColumn> columns);

    /// <summary>
    ///     تولید رشته JOIN query بین جدول پایه و جداول هدف
    ///     - مسیرهای Join با IJoinPathResolver محاسبه می‌شوند
    ///     - فقط جدول‌هایی که هنوز join نشده‌اند اضافه می‌شوند
    ///     - خروجی برای قرار دادن مستقیم در FROM و JOIN query مناسب است
    /// </summary>
    /// <param name="baseTable">نام جدول پایه</param>
    /// <param name="columns">ستون‌های انتخاب شده که جداول مرتبط را مشخص می‌کنند</param>
    /// <param name="getEntityType">Func برای گرفتن IEntityType بر اساس نام جدول</param>
    /// <returns>رشته SQL بخش JOIN</returns>
    string BuildJoinClause(string baseTable, List<SelectedColumn> columns, Func<string, IEntityType> getEntityType);
}