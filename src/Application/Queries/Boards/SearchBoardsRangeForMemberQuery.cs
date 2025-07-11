using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record SearchBoardsRangeForMemberQuery(Guid UserId, string SearchTerm, int Page, int PageSize) : IRequest<IEnumerable<BoardSearchDto>>;
