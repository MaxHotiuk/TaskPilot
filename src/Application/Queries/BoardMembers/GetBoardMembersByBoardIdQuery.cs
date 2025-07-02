using Application.Common.Dtos.BoardMembers;
using MediatR;

namespace Application.Queries.BoardMembers;

public record GetBoardMembersByBoardIdQuery(Guid BoardId) : IRequest<IEnumerable<BoardMemberDto>>;
