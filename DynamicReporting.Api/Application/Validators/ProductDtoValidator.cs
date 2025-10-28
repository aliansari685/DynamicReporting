namespace DynamicReporting.Api.Application.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("نام محصول الزامی است.")
            .MaximumLength(100).WithMessage("نام محصول نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("دسته‌بندی نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("قیمت محصول الزامی است.")
            .GreaterThanOrEqualTo(0).WithMessage("قیمت محصول نمی‌تواند منفی باشد.");

        RuleFor(x => x.Stock)
            .NotNull().WithMessage("موجودی محصول الزامی است.")
            .GreaterThanOrEqualTo(0).WithMessage("موجودی محصول نمی‌تواند منفی باشد.");

        RuleFor(x => x.SupplierId)
            .NotNull().WithMessage("شناسه تأمین‌کننده الزامی است.");

        RuleFor(x => x.CreatedDate)
            .NotNull().WithMessage("تاریخ ایجاد محصول الزامی است.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("تاریخ ایجاد محصول نمی‌تواند در آینده باشد.");

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("وضعیت موجود بودن محصول الزامی است.");
    }
}