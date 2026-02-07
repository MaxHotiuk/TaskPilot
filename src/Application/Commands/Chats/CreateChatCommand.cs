using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public record CreateChatCommand(
    Guid OrganizationId,
    Guid CreatedById,
    ChatType Type,
    string? Name,
    IEnumerable<Guid> MemberIds
) : IRequest<Guid>;
