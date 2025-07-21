using FluentValidation;

namespace Application.Validators.Commands.States;

public class SwapStateOrderCommandValidator : AbstractValidator<Application.Commands.States.SwapStateOrderCommand>
{
    public SwapStateOrderCommandValidator()
    {
        RuleFor(x => x.FirstStateId).GreaterThan(0);
        RuleFor(x => x.SecondStateId).GreaterThan(0);
        RuleFor(x => x.BoardId).NotEmpty();
        RuleFor(x => x.FirstStateId)
            .NotEqual(x => x.SecondStateId)
            .WithMessage("FirstStateId and SecondStateId must be different.");
    }
}
