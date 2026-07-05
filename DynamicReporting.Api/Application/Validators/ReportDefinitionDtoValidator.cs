namespace DynamicReporting.Api.Application.Validators;

public class ReportDefinitionDtoValidator : AbstractValidator<ReportDefinitionDto>
{
    public ReportDefinitionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام گزارش اجباری است.")
            .MaximumLength(200).WithMessage("حداکثر طول نام گزارش 200 کاراکتر است.");

        RuleFor(x => x.SelectedColumns)
            .NotNull().WithMessage("لیست ستون‌ها نباید null باشد.")
            .NotEmpty().WithMessage("حداقل یک ستون باید انتخاب شود.");

        RuleFor(x => x.CreatedBy)
            .MaximumLength(100).WithMessage("حداکثر طول نام ایجادکننده 100 کاراکتر است.")
            .When(x => !string.IsNullOrWhiteSpace(x.CreatedBy));

        RuleFor(x => x.BaseTable)
            .NotEmpty().WithMessage("جدول پایه نا معتبر است");
    }
}