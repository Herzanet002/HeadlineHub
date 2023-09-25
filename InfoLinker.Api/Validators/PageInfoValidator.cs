using FluentValidation;
using InfoLinker.Api.Models;

namespace InfoLinker.Api.Validators;

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