namespace DynamicReporting.Api.Application.Validators;

public sealed class ReportValidation(IUnitOfWork uow) : IReportValidation
{
    public void ValidateFilteringColumn(ReportDefinition report, List<FilterCondition> filters)
    {
        foreach (var filterCondition in filters)
        {
            var parts = filterCondition.Field.Split('.', StringSplitOptions.RemoveEmptyEntries);

            var tableName = parts[0];
            var columnName = parts[1];

            var tableExists = report.SelectedColumns.Any(x =>
                string.Equals(x.Table, tableName, StringComparison.OrdinalIgnoreCase));

            if (!tableExists)
                throw new ArgumentException($"جدول '{tableName}' در گزارش وجود ندارد.");

            var entityType = uow.GetTrustEntityType(tableName);

            var propertyExists = entityType.GetProperties().Any(p =>
                string.Equals(p.GetColumnName(), columnName, StringComparison.OrdinalIgnoreCase));

            if (!propertyExists)
                throw new ArgumentException($"ستون '{columnName}' در جدول '{tableName}' وجود ندارد.");
        }
    }

    public void ValidateSortColumn(ReportDefinition report, SortableColumnDto sortColumn)
    {
        // اگر کاربر ستونی ارسال نکرد، بر اساس کلید اصلی مرتب کن
        if (string.IsNullOrWhiteSpace(sortColumn.Column))
        {
            sortColumn.Column = $"{report.BaseTable}.{GetPrimaryKeyColumn(report.BaseTable)}";
            return;
        }

        var parts = sortColumn.Column.Split('.', StringSplitOptions.RemoveEmptyEntries);

        var tableName = parts[0];
        var columnName = parts[1];

        var tableExists = report.SelectedColumns.Any(x =>
            string.Equals(x.Table, tableName, StringComparison.OrdinalIgnoreCase));

        if (!tableExists)
            throw new ArgumentException($"جدول '{tableName}' در گزارش وجود ندارد.");

        var entityType = uow.GetTrustEntityType(tableName);

        var propertyExists = entityType.GetProperties().Any(p =>
            string.Equals(p.GetColumnName(), columnName, StringComparison.OrdinalIgnoreCase));

        if (!propertyExists)
            throw new ArgumentException($"ستون '{columnName}' در جدول '{tableName}' وجود ندارد.");
    }

    #region Helper Method

    /// <summary>
    ///     متد کمکی برای دریافت ستون کلید اصلی
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetPrimaryKeyColumn(string tableName)
    {
        var entityType = uow.GetTrustEntityType(tableName);
        var primaryKey = entityType.FindPrimaryKey();

        return primaryKey == null ? "(SELECT NULL)" :
            primaryKey.Properties.Count == 1 ? primaryKey.Properties.First().GetColumnName() :
            string.Join(", ", primaryKey.Properties.Select(p => $"[{p.GetColumnName()}]"));
    }

    #endregion
}