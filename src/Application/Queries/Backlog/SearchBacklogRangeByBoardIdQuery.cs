using Domain.Dtos.Backlog;
using MediatR;

namespace Application.Queries.Backlog;

public record SearchBacklogRangeByBoardIdQuery(
    Guid BoardId,
    string SearchTerm,
    DateOnly StartDate,
    DateOnly EndDate,
    int Page,
    int PageSize) : IRequest<IEnumerable<BacklogDto>>;
