namespace DynamicReporting.Api.Domain.Interfaces;

public interface IJoinPathResolver
{
    /// <summary>
    ///     محاسبه و بازگردانی کوتاه‌ترین مسیر Join بین دو EntityType
    ///     با استفاده از گراف FKهای EF Core.
    ///     - مسیر فقط روی metadata اجرا می‌شود، نه داده‌ها
    ///     - نتیجه در cache داخلی ذخیره می‌شود تا محاسبات تکراری صفر شود
    ///     - اگر مسیر بین جدول‌ها وجود نداشته باشد، InvalidOperationException پرتاب می‌شود
    /// </summary>
    /// <param name="from">EntityType مبدا</param>
    /// <param name="to">EntityType مقصد</param>
    /// <returns>لیست FKهایی که مسیر Join بین دو جدول را تشکیل می‌دهند</returns>
    List<IReadOnlyForeignKey> Resolve(IEntityType from, IEntityType to);
}