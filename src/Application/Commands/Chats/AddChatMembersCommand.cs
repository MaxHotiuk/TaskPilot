using MediatR;

namespace Application.Commands.Chats;

public record AddChatMembersCommand(Guid ChatId, Guid UserId, IEnumerable<Guid> MemberIds) : IRequest;
