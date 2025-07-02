using Application.Commands.Tasks;
using FluentValidation;

namespace Application.Validators.Commands.Tasks;

public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    public CreateTaskItemCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.")
            .MinimumLength(1).WithMessage("Task title must be at least 1 character.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Task description must not exceed 2000 characters.");

        RuleFor(x => x.StateId)
            .GreaterThan(0).WithMessage("State ID must be greater than 0.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Due date cannot be in the past.")
            .When(x => x.DueDate.HasValue);
    }
}
