using Application.Commands.Chats;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class ClearChatHistoryCommandValidator : AbstractValidator<ClearChatHistoryCommand>
{
    public ClearChatHistoryCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
