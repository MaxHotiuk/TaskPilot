using Application.Commands.Chats;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class AddChatMembersCommandValidator : AbstractValidator<AddChatMembersCommand>
{
    public AddChatMembersCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.MemberIds)
            .NotEmpty().WithMessage("Member IDs are required.");
    }
}
