using MediatR;

namespace Application.Commands.Chats;

public record DeleteChatCommand(Guid ChatId, Guid UserId) : IRequest;
