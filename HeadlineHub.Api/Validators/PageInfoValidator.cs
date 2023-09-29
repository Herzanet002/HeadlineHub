using FluentValidation;
using HeadlineHub.Api.Models;

namespace HeadlineHub.Api.Validators;

public class PageInfoValidator : AbstractValidator<PageInfo>
{
    public PageInfoValidator()
    {
        RuleFor(info => info.Index)
            .GreaterThanOrEqualTo(0);
        RuleFor(info => info.Size)
            .GreaterThan(0);
    }
}