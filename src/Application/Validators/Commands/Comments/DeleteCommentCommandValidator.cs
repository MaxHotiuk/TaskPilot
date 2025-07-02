using Application.Commands.Comments;
using FluentValidation;

namespace Application.Validators.Commands.Comments;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Comment ID is required.");
    }
}
