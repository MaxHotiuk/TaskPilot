using Application.Commands.UserProfile;
using FluentValidation;

namespace Application.Validators.Commands.UserProfile;

public class DeleteUserProfileCommandValidator : AbstractValidator<DeleteUserProfileCommand>
{
    public DeleteUserProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
