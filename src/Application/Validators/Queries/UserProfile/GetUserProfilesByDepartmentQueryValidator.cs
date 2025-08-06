using Application.Queries.UserProfile;
using FluentValidation;

namespace Application.Validators.Queries.UserProfile;

public class GetUserProfilesByDepartmentQueryValidator : AbstractValidator<GetUserProfilesByDepartmentQuery>
{
    public GetUserProfilesByDepartmentQueryValidator()
    {
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");
    }
}
