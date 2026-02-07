using MediatR;

namespace Application.Commands.Chats;

public record UpdateChatNameCommand(Guid ChatId, Guid UserId, string Name) : IRequest;
