using Application.Abstractions.Persistence;
using Domain.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using Domain.Dtos.Backlog;

namespace Application.Queries.Backlog;

public class SearchBacklogRangeByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<SearchBacklogRangeByBoardIdQuery, IEnumerable<BacklogDto>>
{
    public SearchBacklogRangeByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BacklogDto>> Handle(SearchBacklogRangeByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Backlogs.SearchBacklogsForBoardAsync(
                request.BoardId,
                request.SearchTerm,
                request.Page,
                request.PageSize,
                request.StartDate,
                request.EndDate,
                cancellationToken);
        }, cancellationToken);
    }
}
