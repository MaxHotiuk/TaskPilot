using Application.Common.Dtos.BoardMembers;
using MediatR;

namespace Application.Queries.BoardMembers;

public record GetBoardMembersByUserIdQuery(Guid UserId) : IRequest<IEnumerable<BoardMemberDto>>;
