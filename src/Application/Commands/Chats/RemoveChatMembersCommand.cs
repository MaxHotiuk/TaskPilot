using MediatR;

namespace Application.Commands.Chats;

public record RemoveChatMembersCommand(Guid ChatId, Guid UserId, IEnumerable<Guid> MemberIds) : IRequest;
