namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

public interface IFilterOperatorHelper
{
    public List<FilterOperatorInfo> GetSupportedOperators(string dataType);
}