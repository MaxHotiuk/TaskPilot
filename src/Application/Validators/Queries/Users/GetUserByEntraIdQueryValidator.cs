using Application.Queries.Users;
using FluentValidation;

namespace Application.Validators.Queries.Users;

public class GetUserByEntraIdQueryValidator : AbstractValidator<GetUserByEntraIdQuery>
{
    public GetUserByEntraIdQueryValidator()
    {
        RuleFor(x => x.EntraId)
            .NotEmpty().WithMessage("EntraId is required.")
            .MaximumLength(100).WithMessage("EntraId must not exceed 100 characters.");
    }
}
