using DynamicReporting.Api.Application.DTOs;
using DynamicReporting.Api.Application.Validators;
using DynamicReporting.Api.Domain.Models;
using FluentValidation.TestHelper;

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
      //      SelectedColumns = new() { "OrderId", "Price" }
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
