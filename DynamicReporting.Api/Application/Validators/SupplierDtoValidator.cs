namespace DynamicReporting.Api.Application.Validators;

public class SupplierDtoValidator : AbstractValidator<SupplierDto>
{
    public SupplierDtoValidator()
    {
        RuleFor(x => x.SupplierName)
            .NotEmpty().WithMessage("نام تأمین‌کننده الزامی است.")
            .MaximumLength(100).WithMessage("نام تأمین‌کننده نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.City)
            .MaximumLength(50).WithMessage("نام شهر نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.Country)
            .MaximumLength(50).WithMessage("نام کشور نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.ContactName)
            .MaximumLength(100).WithMessage("نام تماس نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.Phone)
            .MaximumLength(30).WithMessage("شماره تلفن نمی‌تواند بیش از ۳۰ کاراکتر باشد.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("ایمیل وارد شده معتبر نیست.")
            .MaximumLength(100).WithMessage("ایمیل نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.RegisterDate)
            .NotNull().WithMessage("تاریخ ثبت نام الزامی است.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("تاریخ ثبت نام نمی‌تواند در آینده باشد.");
    }
}