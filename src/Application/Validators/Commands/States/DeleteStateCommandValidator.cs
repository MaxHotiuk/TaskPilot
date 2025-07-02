using Application.Commands.States;
using FluentValidation;

namespace Application.Validators.Commands.States;

public class DeleteStateCommandValidator : AbstractValidator<DeleteStateCommand>
{
    public DeleteStateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("State ID must be greater than 0.");
    }
}
