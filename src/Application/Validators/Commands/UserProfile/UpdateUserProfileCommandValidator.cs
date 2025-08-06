using Application.Commands.UserProfile;
using FluentValidation;

namespace Application.Validators.Commands.UserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(x => x.Bio != null);

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.")
            .When(x => x.JobTitle != null);

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Department != null);

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters.")
            .When(x => x.Location != null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .When(x => x.PhoneNumber != null);
    }
}
