using MediatR;

namespace Application.Queries.BoardMembers;

public record CheckBoardMembershipQuery(Guid BoardId, Guid UserId) : IRequest<bool>;
