using Application.Commands.BoardMembers;
using Domain.Common.Authorization;
using FluentValidation;

namespace Application.Validators.Commands.BoardMembers;

public class UpdateBoardMemberRoleCommandValidator : AbstractValidator<UpdateBoardMemberRoleCommand>
{
    public UpdateBoardMemberRoleCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(50).WithMessage("Role must not exceed 50 characters.")
            .Must(role => BoardMemberRoles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", BoardMemberRoles.All)}.");
    }
}
