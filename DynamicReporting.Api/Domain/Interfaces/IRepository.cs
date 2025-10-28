namespace DynamicReporting.Api.Domain.Interfaces
{
    /// <summary>
    ///     رابط عمومی برای عملیات پایه‌ی CRUD روی موجودیت‌های سیستم
    /// </summary>
    /// <typeparam name="T"> مدلی که ریپازیتوری با آن کار می‌کند</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        ///     افزودن موجودیت جدید به مخزن داده
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر برای افزودن</param>
        void Add(T entity);

        /// <summary>
        ///     به‌روزرسانی موجودیت در مخزن داده
        /// </summary>
        /// <param name="entity">موجودیت با مقادیر ویرایش شده</param>
        void Update(T entity);

        /// <summary>
        ///     حذف موجودیت از مخزن داده
        /// </summary>
        /// <param name="entity">موجودیت مورد نظر برای حذف</param>
        void Remove(T entity);

        /// <summary>
        ///     بازیابی موجودیت بر اساس شناسه منحصربه‌فرد
        /// </summary>
        /// <param name="id">شناسه موجودیت مورد نظر</param>
        /// <returns>موجودیت مربوطه یا null اگر یافت نشود</returns>
        T GetById(int id);

        /// <summary>
        ///     بازیابی تمامی موجودیت‌های نوع T از مخزن داده
        /// </summary>
        /// <returns>لیستی از تمام موجودیت‌ها - لیست خالی اگر هیچ موردی وجود نداشته باشد</returns>
        List<T> GetAllToList();

        /// <summary>
        ///     بازیابی تمامی موجودیت‌های نوع T از مخزن داده
        /// </summary>
        /// <returns>لیستی از تمام موجودیت‌ها - لیست خالی اگر هیچ موردی وجود نداشته باشد</returns>
        IQueryable<T> GetAll();

        /// <summary>
        ///     بازیابی موجودیت بر اساس مقدار یکی از خصوصیات آن
        /// </summary>
        /// <param name="predicate">مقدار مورد جستجو در خصوصیت</param>
        /// <returns>موجودیت مربوطه یا null اگر یافت نشود</returns>
        T GetByProperty(Expression<Func<T, bool>> predicate);
    }
}