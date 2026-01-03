namespace Tests.Application.Tests.Validators;

public class ReportDefinitionDtoValidatorTests
{
    private readonly ReportDefinitionDtoValidator _validator = new();

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var dto = new ReportDefinitionDto { Name = "" };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_pass_when_data_is_valid()
    {
        var dto = new ReportDefinitionDto
        {
            Name = "Sales Report",
            SelectedColumns =
            [
                new() { Table = "Customers", Column = "FullName" },
                new() { Table = "Customers", Column = "City" },
                new() { Table = "Customers", Column = "Country" },
                new() { Table = "Orders", Column = "OrderDate" },
                new() { Table = "Orders", Column = "Status" },
                new() { Table = "Orders", Column = "TotalAmount" },
                new() { Table = "OrderItems", Column = "Quantity" },
                new() { Table = "OrderItems", Column = "Total" }
            ],
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
