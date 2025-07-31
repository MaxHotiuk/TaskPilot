using Application.Abstractions.Persistence;
using Domain.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class SearchArchivedRangeTaskItemsQueryHandler : BaseQueryHandler, IRequestHandler<SearchArchivedRangeTaskItemsQuery, IEnumerable<ArchivedTaskDto>>
{
    public SearchArchivedRangeTaskItemsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ArchivedTaskDto>> Handle(SearchArchivedRangeTaskItemsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Tasks.SearchArchivedRangeByBoardIdAsync(request.BoardId, request.Page, request.PageSize, request.SearchTerm, cancellationToken);
        }, cancellationToken);
    }
}
