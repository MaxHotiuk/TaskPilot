using MediatR;

namespace Application.Commands.Chats;

public record UpdateChatReadStatusCommand(Guid ChatId, Guid UserId, DateTime ReadAt) : IRequest;
