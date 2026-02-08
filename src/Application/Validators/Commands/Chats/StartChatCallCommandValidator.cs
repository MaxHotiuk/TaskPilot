using Application.Commands.Chats;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class StartChatCallCommandValidator : AbstractValidator<StartChatCallCommand>
{
    public StartChatCallCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required.");
    }
}
