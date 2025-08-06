using Application.Commands.UserProfile;
using FluentValidation;

namespace Application.Validators.Commands.UserProfile;

public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.JobTitle));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Department));

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
