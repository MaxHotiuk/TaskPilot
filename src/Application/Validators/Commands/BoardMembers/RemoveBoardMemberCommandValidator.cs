using Application.Commands.BoardMembers;
using FluentValidation;

namespace Application.Validators.Commands.BoardMembers;

public class RemoveBoardMemberCommandValidator : AbstractValidator<RemoveBoardMemberCommand>
{
    public RemoveBoardMemberCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
