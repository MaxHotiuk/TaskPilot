using MediatR;
using Domain.Dtos.Chats;

namespace Application.Commands.Chats;

public record SendChatMessageCommand(
    Guid ChatId,
    Guid SenderId,
    string Content
) : IRequest<ChatMessageDto>;
