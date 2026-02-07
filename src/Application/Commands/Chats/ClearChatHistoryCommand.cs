using MediatR;

namespace Application.Commands.Chats;

public record ClearChatHistoryCommand(Guid ChatId, Guid UserId) : IRequest;
