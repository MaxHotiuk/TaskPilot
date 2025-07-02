using Application.Commands.Boards;
using FluentValidation;

namespace Application.Validators.Commands.Boards;

public class DeleteBoardCommandValidator : AbstractValidator<DeleteBoardCommand>
{
    public DeleteBoardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Board ID is required.");
    }
}
