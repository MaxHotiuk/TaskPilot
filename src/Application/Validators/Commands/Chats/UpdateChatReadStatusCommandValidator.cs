using Application.Commands.Chats;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class UpdateChatReadStatusCommandValidator : AbstractValidator<UpdateChatReadStatusCommand>
{
    public UpdateChatReadStatusCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ReadAt)
            .NotEmpty().WithMessage("Read timestamp is required.");
    }
}
