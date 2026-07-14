namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

public sealed class FilterOperatorHelper : IFilterOperatorHelper
{
    private readonly Dictionary<string, List<FilterOperatorInfo>> _operatorMap = new()
    {
        // انواع عددی
        {
            "int", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "int32", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "long", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "decimal", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "double", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "float", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بزرگتر" },
                new() { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new() { Operator = "lt", DisplayName = "کوچکتر" },
                new() { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },

        // انواع رشته‌ای
        {
            "string", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "contains", DisplayName = "شامل" },
                new() { Operator = "startswith", DisplayName = "شروع با" },
                new() { Operator = "endswith", DisplayName = "پایان با" }
            ]
        },

        // انواع تاریخ
        {
            "datetime", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بعد از" },
                new() { Operator = "gte", DisplayName = "از تاریخ به بعد" },
                new() { Operator = "lt", DisplayName = "قبل از" },
                new() { Operator = "lte", DisplayName = "تا تاریخ" }
            ]
        },
        {
            "datetime2", [
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بعد از" },
                new() { Operator = "gte", DisplayName = "از تاریخ به بعد" },
                new() { Operator = "lt", DisplayName = "قبل از" },
                new() { Operator = "lte", DisplayName = "تا تاریخ" }
            ]
        },
        {
            "dateonly", new List<FilterOperatorInfo>
            {
                new() { Operator = "eq", DisplayName = "برابر" },
                new() { Operator = "gt", DisplayName = "بعد از" },
                new() { Operator = "gte", DisplayName = "از تاریخ به بعد" },
                new() { Operator = "lt", DisplayName = "قبل از" },
                new() { Operator = "lte", DisplayName = "تا تاریخ" }
            }
        },

        // انواع بولی
        {
            "bool", [new() { Operator = "eq", DisplayName = "برابر" }]
        },
        {
            "boolean", [new() { Operator = "eq", DisplayName = "برابر" }]
        },

        // انواع Guid
        {
            "guid", [new() { Operator = "eq", DisplayName = "برابر" }]
        }
    };

    public List<FilterOperatorInfo> GetSupportedOperators(string dataType)
    {
        var key = dataType.ToLowerInvariant();

        if (_operatorMap.TryGetValue(key, out var operators))
            return operators;

        if (key.EndsWith("?"))
        {
            var baseType = key.TrimEnd('?');
            if (_operatorMap.TryGetValue(baseType, out var supportedOperators))
                return supportedOperators;
        }

        return _operatorMap["string"];
    }
}