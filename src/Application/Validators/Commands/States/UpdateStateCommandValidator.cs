using Application.Commands.States;
using FluentValidation;

namespace Application.Validators.Commands.States;

public class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("State ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("State name is required.")
            .MaximumLength(100).WithMessage("State name must not exceed 100 characters.")
            .MinimumLength(1).WithMessage("State name must be at least 1 character.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0.");
    }
}
