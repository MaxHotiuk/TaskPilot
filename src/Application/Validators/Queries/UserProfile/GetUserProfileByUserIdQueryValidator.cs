using Application.Queries.UserProfile;
using FluentValidation;

namespace Application.Validators.Queries.UserProfile;

public class GetUserProfileByUserIdQueryValidator : AbstractValidator<GetUserProfileByUserIdQuery>
{
    public GetUserProfileByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
