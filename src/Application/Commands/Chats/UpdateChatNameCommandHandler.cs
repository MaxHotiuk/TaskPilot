using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public class UpdateChatNameCommandHandler : BaseCommandHandler, IRequestHandler<UpdateChatNameCommand>
{
    private readonly IChatNotifier _chatNotifier;

    public UpdateChatNameCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
    }

    public async Task Handle(UpdateChatNameCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var chat = await unitOfWork.Chats.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
            {
                throw new ValidationException($"Chat with ID {request.ChatId} does not exist");
            }

            if (chat.Type != ChatType.Group)
            {
                throw new ValidationException("Only group chats can be renamed.");
            }

            var owner = await unitOfWork.ChatMembers.GetMemberAsync(request.ChatId, request.UserId, cancellationToken);
            if (owner is null || owner.Role != ChatMemberRole.Owner)
            {
                throw new ValidationException("Only chat owners can rename the chat.");
            }

            chat.Name = request.Name.Trim();
            chat.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Chats.Update(chat);

            var chatMembers = await unitOfWork.ChatMembers.GetMembersAsync(request.ChatId, cancellationToken);
            var chatDto = new ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = null,
                Members = chatMembers.Select(chatMember => chatMember.ToDto()).ToList()
            };

            await _chatNotifier.NotifyChatUpdatedAsync(chatMembers.Select(chatMember => chatMember.UserId), chatDto);
        }, cancellationToken);
    }
}
