using DynamicReporting.Api.Shared.Helper;

namespace DynamicReporting.Api.Application.Services;

public class ReportMetadataService
{
    public ReportMetadataService()
    {
        var test = ExtensionMethod.GetPropertyNames<CustomerDto>();
        test.AddRange(ExtensionMethod.GetPropertyNames<OrderDto>());
        test.AddRange(ExtensionMethod.GetPropertyNames<OrderItemDto>());
        test.AddRange(ExtensionMethod.GetPropertyNames<ProductDto>());
        test.AddRange(ExtensionMethod.GetPropertyNames<SupplierDto>());
    }
}