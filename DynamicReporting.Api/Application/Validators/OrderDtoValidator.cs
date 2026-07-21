namespace DynamicReporting.Api.Application.Validators;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotNull().WithMessage("شناسه مشتری الزامی است.");

        RuleFor(x => x.OrderDate)
            .NotNull().WithMessage("تاریخ سفارش الزامی است.")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("تاریخ سفارش نمی‌تواند در آینده باشد.");

        RuleFor(x => x.TotalAmount)
            .NotNull().WithMessage("مبلغ کل الزامی است.")
            .GreaterThan(0).WithMessage("مبلغ کل باید بیشتر از صفر باشد.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("وضعیت سفارش الزامی است.")
            .MaximumLength(20).WithMessage("وضعیت سفارش حداکثر باید ۲۰ کاراکتر باشد.");

        RuleFor(x => x.PaymentType)
            .NotEmpty().WithMessage("نوع پرداخت الزامی است.")
            .MaximumLength(20).WithMessage("نوع پرداخت حداکثر باید ۲۰ کاراکتر باشد.");

        RuleFor(x => x.ShippingCity)
            .MaximumLength(50).WithMessage("شهر ارسال حداکثر باید ۵۰ کاراکتر باشد.");

        RuleFor(x => x.ShippingCountry)
            .MaximumLength(50).WithMessage("کشور ارسال حداکثر باید ۵۰ کاراکتر باشد.");
    }
}