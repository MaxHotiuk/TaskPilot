using Domain.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record SearchBoardsRangeForOwnerQuery(Guid OwnerId, string SearchTerm, int Page, int PageSize) : IRequest<IEnumerable<BoardSearchDto>>;
