namespace DynamicReporting.Api.Domain.Interfaces;

public interface IBaseTableResolver
{
    /// <summary>
    /// این کلاس جدول پایه را برای کوئری ها پیدا میکند با توجه ب فارن کی و جوین ها
    /// </summary>
    string Resolve(List<SelectedColumn> columns);
}