namespace DynamicReporting.Api.Infrastructure.Persistence.Query;

public sealed class FilterOperatorHelper : IFilterOperatorHelper
{
    private readonly Dictionary<string, List<FilterOperatorInfo>> _operatorMap = new()
    {
        // انواع عددی
        {
            "int", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "int32", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "long", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "decimal", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "double", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },
        {
            "float", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بزرگتر" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "بزرگتر یا مساوی" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "کوچکتر" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "کوچکتر یا مساوی" }
            ]
        },

        // انواع رشته‌ای
        {
            "string", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "contains", DisplayName = "شامل" }
            ]
        },

        // انواع تاریخ
        {
            "datetime", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بعد از" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "از تاریخ به بعد" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "قبل از" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "تا تاریخ" }
            ]
        },
        {
            "datetime2", [
                new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" },
                new FilterOperatorInfo { Operator = "gt", DisplayName = "بعد از" },
                new FilterOperatorInfo { Operator = "gte", DisplayName = "از تاریخ به بعد" },
                new FilterOperatorInfo { Operator = "lt", DisplayName = "قبل از" },
                new FilterOperatorInfo { Operator = "lte", DisplayName = "تا تاریخ" }
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
            "bool", [new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" }]
        },
        {
            "boolean", [new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" }]
        },

        // انواع Guid
        {
            "guid", [new FilterOperatorInfo { Operator = "eq", DisplayName = "برابر" }]
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