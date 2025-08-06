using Application.Queries.UserProfile;
using FluentValidation;

namespace Application.Validators.Queries.UserProfile;

public class GetUserProfileByIdQueryValidator : AbstractValidator<GetUserProfileByIdQuery>
{
    public GetUserProfileByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
