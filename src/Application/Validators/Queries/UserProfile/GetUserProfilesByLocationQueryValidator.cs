using Application.Queries.UserProfile;
using FluentValidation;

namespace Application.Validators.Queries.UserProfile;

public class GetUserProfilesByLocationQueryValidator : AbstractValidator<GetUserProfilesByLocationQuery>
{
    public GetUserProfilesByLocationQueryValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");
    }
}
