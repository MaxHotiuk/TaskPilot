using Application.Commands.States;
using FluentValidation;

namespace Application.Validators.Commands.States;

public class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("State name is required.")
            .MaximumLength(100).WithMessage("State name must not exceed 100 characters.")
            .MinimumLength(1).WithMessage("State name must be at least 1 character.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0.");
    }
}
