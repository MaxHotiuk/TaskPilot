using Application.Abstractions.Meetings;
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Chats;

public class StartChatCallCommandHandler : BaseCommandHandler, IRequestHandler<StartChatCallCommand, StartChatCallResponseDto>
{
    private readonly IChatNotifier _chatNotifier;
    private readonly IDailyRoomService _dailyRoomService;

    public StartChatCallCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IChatNotifier chatNotifier,
        IDailyRoomService dailyRoomService)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
        _dailyRoomService = dailyRoomService;
    }

    public async Task<StartChatCallResponseDto> Handle(StartChatCallCommand request, CancellationToken cancellationToken)
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

            var callId = Guid.NewGuid();
            var roomUrl = await _dailyRoomService.CreateRoomAsync(callId, cancellationToken);

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatId = request.ChatId,
                SenderId = request.SenderId,
                Sender = sender,
                Content = roomUrl,
                MessageType = "Call",
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
            var chatDto = new ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
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

            return new StartChatCallResponseDto
            {
                RoomUrl = roomUrl,
                Message = messageDto
            };
        }, cancellationToken);
    }
}
