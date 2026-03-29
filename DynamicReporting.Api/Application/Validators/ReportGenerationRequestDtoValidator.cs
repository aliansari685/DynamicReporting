namespace DynamicReporting.Api.Application.Validators;

public class ReportGenerationRequestDtoValidator : AbstractValidator<ReportGenerationRequestDto>
{
    public ReportGenerationRequestDtoValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("شناسه جاب الزامی است و نباید خالی باشد.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).When(x => x.UserId.HasValue)
            .WithMessage("در صورت وارد شدن، شناسه کاربر باید بزرگ‌تر از صفر باشد.");
    }
}