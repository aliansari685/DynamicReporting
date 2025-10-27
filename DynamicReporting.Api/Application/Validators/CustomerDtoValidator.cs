namespace DynamicReporting.Api.Application.Validators;

public class CustomerDtoValidator : AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("نام کامل مشتری الزامی است").MaximumLength(100);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("ایمیل مشتری الزامی است").MaximumLength(100);

        RuleFor(x => x.Phone).MaximumLength(30);
    }
}