using MediatR;

namespace Application.Commands.BoardMembers;

public record AddBoardMemberCommand(
    Guid BoardId,
    Guid UserId,
    string Role = "Member"
) : IRequest;
