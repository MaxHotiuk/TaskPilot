using Domain.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record SearchBoardsRangeForUserQuery(Guid UserId, string SearchTerm, int Page, int PageSize) : IRequest<IEnumerable<BoardSearchDto>>;
