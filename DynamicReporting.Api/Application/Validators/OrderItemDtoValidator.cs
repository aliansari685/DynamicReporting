namespace DynamicReporting.Api.Application.Validators;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotNull().WithMessage("شناسه سفارش الزامی است.");

        RuleFor(x => x.ProductId)
            .NotNull().WithMessage("شناسه محصول الزامی است.");

        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("تعداد الزامی است.")
            .GreaterThan(0).WithMessage("تعداد باید بیشتر از صفر باشد.");

        RuleFor(x => x.UnitPrice)
            .NotNull().WithMessage("قیمت واحد الزامی است.")
            .GreaterThan(0).WithMessage("قیمت واحد باید بیشتر از صفر باشد.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("تخفیف نمی‌تواند منفی باشد.")
            .LessThanOrEqualTo(100).WithMessage("تخفیف نمی‌تواند بیشتر از ۱۰۰ درصد باشد.");

        RuleFor(x => x.Total)
            .NotNull().WithMessage("مبلغ کل الزامی است.")
            .GreaterThan(0).WithMessage("مبلغ کل باید بیشتر از صفر باشد.");
    }
}