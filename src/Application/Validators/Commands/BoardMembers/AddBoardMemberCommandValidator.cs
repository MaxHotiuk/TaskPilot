using Application.Commands.BoardMembers;
using FluentValidation;

namespace Application.Validators.Commands.BoardMembers;

public class AddBoardMemberCommandValidator : AbstractValidator<AddBoardMemberCommand>
{
    public AddBoardMemberCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(50).WithMessage("Role must not exceed 50 characters.")
            .Must(role => new[] { "Owner", "Admin", "Member" }.Contains(role))
            .WithMessage("Role must be one of: Owner, Admin, Member.");
    }
}
