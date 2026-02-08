using Application.Commands.Chats;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class UpdateChatNameCommandValidator : AbstractValidator<UpdateChatNameCommand>
{
    public UpdateChatNameCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Chat name is required.")
            .MaximumLength(200).WithMessage("Chat name must not exceed 200 characters.");
    }
}
