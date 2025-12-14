namespace DynamicReporting.Api.Domain.Interfaces;

public interface IBaseTableResolver
{
    string Resolve(List<SelectedColumn> columns);
}