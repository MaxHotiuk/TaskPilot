using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Chats;

public class SendChatMessageCommandHandler : BaseCommandHandler, IRequestHandler<SendChatMessageCommand, ChatMessageDto>
{
    private readonly IChatNotifier _chatNotifier;

    public SendChatMessageCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
    }

    public async Task<ChatMessageDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var chat = await unitOfWork.Chats.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
            {
                throw new ValidationException($"Chat with ID {request.ChatId} does not exist");
            }

            if (!await unitOfWork.ChatMembers.IsMemberAsync(request.ChatId, request.SenderId, cancellationToken))
            {
                throw new ValidationException("User is not a member of this chat.");
            }

            var sender = await unitOfWork.Users.GetByIdAsync(request.SenderId, cancellationToken);
            if (sender is null)
            {
                throw new ValidationException($"User with ID {request.SenderId} does not exist");
            }

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatId = request.ChatId,
                SenderId = request.SenderId,
                Sender = sender,
                Content = request.Content.Trim(),
                MessageType = "Text",
                HasAttachments = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.ChatMessages.AddAsync(message, cancellationToken);

            chat.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Chats.Update(chat);

            var messageDto = message.ToDto();

            var memberIds = await unitOfWork.ChatMembers.GetMemberIdsAsync(request.ChatId, cancellationToken);
            var chatMembers = await unitOfWork.ChatMembers.GetMembersAsync(request.ChatId, cancellationToken);
            var chatDto = new Domain.Dtos.Chats.ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                BoardId = chat.BoardId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = message.ToPreviewDto(),
                Members = chatMembers.Select(member => member.ToDto()).ToList()
            };

            await _chatNotifier.NotifyChatMessageAsync(request.ChatId, messageDto);
            await _chatNotifier.NotifyChatUpdatedAsync(memberIds, chatDto);

            return messageDto;
        }, cancellationToken);
    }
}
