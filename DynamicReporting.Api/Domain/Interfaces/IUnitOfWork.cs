using DynamicReporting.Api.Infrastructure.Persistence.DbContext;

namespace DynamicReporting.Api.Domain.Interfaces;

/// <summary>
/// کلاس یونیت آف ورک یا همون کنترل واحد
/// مدیریت تراکنش‌ها و دسترسی به ریپازیتوری ها به صورت مرکزی و یکپارچه (جنریک)
/// نال در همه ی موارد چک شده
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// فراخوانی و بارگذاری مدل اصلی و کش آن
    /// </summary>
    /// <typeparam name="T">نوع موجودیت</typeparam>
    /// <returns></returns>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// متد اغاز تراکنش بصورت کانکارنسی
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// ذخیره عملیات و بررسی تراکنش و درصورت وجود مشکل رولبک میشه عملیات
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    /// کانتکسم صرفا جهت استفاده برای گرفتن دیتا
    /// </summary>
    ShopTestDbContext DbContext { get; }
}