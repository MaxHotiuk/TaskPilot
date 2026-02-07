using Application.Commands.Chats;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Commands.Chats;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required.");

        RuleFor(x => x.CreatedById)
            .NotEmpty().WithMessage("Creator ID is required.");

        RuleFor(x => x.MemberIds)
            .NotEmpty().WithMessage("At least one member is required.");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Chat name must not exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Chat type is invalid.");

        When(x => x.Type == ChatType.Group, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Group chat name is required.");
        });
    }
}
