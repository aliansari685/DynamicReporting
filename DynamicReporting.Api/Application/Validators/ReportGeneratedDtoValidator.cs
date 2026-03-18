namespace DynamicReporting.Api.Application.Validators;

public class ReportGeneratedDtoValidator : AbstractValidator<ReportGeneratedDto>
{
    public ReportGeneratedDtoValidator()
    {
        RuleFor(x => x.ReportGuid)
            .NotEmpty().WithMessage("شناسه گزارش الزامی است و نباید خالی باشد.");

        RuleFor(x => x.DownloadUrl)
            .NotEmpty().WithMessage("آدرس لینک دانلود فایل الزامی است.")
            .MaximumLength(500).WithMessage("آدرس لینک نمی‌تواند بیش از ۵۰۰ کاراکتر باشد.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("فرمت آدرس لینک دانلود معتبر نیست، لطفاً یک لینک صحیح وارد کنید.");

        RuleFor(x => x.DateTime)
            .NotEmpty().WithMessage("زمان انقضا الزامی است.")
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("زمان انقضا باید بزرگ‌تر از زمان فعلی باشد.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).When(x => x.UserId.HasValue)
            .WithMessage("در صورت وارد شدن، شناسه کاربر باید بزرگ‌تر از صفر باشد.");
    }
}