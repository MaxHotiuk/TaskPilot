using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class SearchBoardsRangeForOwnerQueryHandler : BaseQueryHandler, IRequestHandler<SearchBoardsRangeForOwnerQuery, IEnumerable<BoardSearchDto>>
{
    public SearchBoardsRangeForOwnerQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardSearchDto>> Handle(SearchBoardsRangeForOwnerQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.SearchBoardsRangeForOwnerAsync(request.OwnerId, request.SearchTerm, request.Page, request.PageSize, cancellationToken);
            return boards.ToSearchDto().OrderBy(b => b.Name);
        }, cancellationToken);
    }
}
