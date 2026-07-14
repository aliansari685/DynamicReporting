namespace DynamicReporting.Api.Domain.Interfaces;

public interface IFilterOperatorHelper
{
    /// <summary>
    /// گرفتن نوع عملیات ها برای فیلتر کردن ستون ها
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public List<FilterOperatorInfo> GetSupportedOperators(string dataType);
}