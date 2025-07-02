using MediatR;

namespace Application.Commands.BoardMembers;

public record RemoveBoardMemberCommand(
    Guid BoardId,
    Guid UserId
) : IRequest;
