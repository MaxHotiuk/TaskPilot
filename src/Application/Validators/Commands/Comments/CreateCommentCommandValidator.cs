using Application.Commands.Comments;
using FluentValidation;

namespace Application.Validators.Commands.Comments;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Comment content must not exceed 1000 characters.")
            .MinimumLength(1).WithMessage("Comment content must be at least 1 character.");
    }
}
