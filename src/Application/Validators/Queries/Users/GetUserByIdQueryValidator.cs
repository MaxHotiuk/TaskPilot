using Application.Queries.Users;
using FluentValidation;

namespace Application.Validators.Queries.Users;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User Id is required.");
    }
}
