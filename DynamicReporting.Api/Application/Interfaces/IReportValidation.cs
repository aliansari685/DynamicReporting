namespace DynamicReporting.Api.Application.Interfaces;

public interface IReportValidation
{
    /// <summary>
    ///     ولیدیشن ستون و جدول ارسالی جهت فیلتر کردن
    /// </summary>
    /// <param name="report"></param>
    /// <param name="filters"></param>
    public void ValidateFilteringColumn(ReportDefinition report, List<FilterCondition> filters);

    /// <summary>
    ///     ولیدیشن ستون و جدول ارسالی جهت مرتب سازی
    /// </summary>
    /// <param name="report"></param>
    /// <param name="sortColumn"></param>
    /// <exception cref="ArgumentException"></exception>
    public void ValidateSortColumn(ReportDefinition report, SortableColumnDto sortColumn);
}