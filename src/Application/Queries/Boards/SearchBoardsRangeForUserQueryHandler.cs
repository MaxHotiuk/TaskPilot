using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class SearchBoardsRangeForUserQueryHandler : BaseQueryHandler, IRequestHandler<SearchBoardsRangeForUserQuery, IEnumerable<BoardSearchDto>>
{
    public SearchBoardsRangeForUserQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardSearchDto>> Handle(SearchBoardsRangeForUserQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.SearchBoardsRangeForUserAsync(request.UserId, request.SearchTerm, request.Page, request.PageSize, cancellationToken);
            return boards.ToSearchDto().OrderBy(b => b.Name);
        }, cancellationToken);
    }
}
