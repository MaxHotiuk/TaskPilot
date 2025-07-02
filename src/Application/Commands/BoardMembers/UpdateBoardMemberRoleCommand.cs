using MediatR;

namespace Application.Commands.BoardMembers;

public record UpdateBoardMemberRoleCommand(
    Guid BoardId,
    Guid UserId,
    string Role
) : IRequest;
