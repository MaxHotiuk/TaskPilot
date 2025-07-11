using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class SearchBoardsRangeForUserMemberHandler : BaseQueryHandler, IRequestHandler<SearchBoardsRangeForMemberQuery, IEnumerable<BoardSearchDto>>
{
    public SearchBoardsRangeForUserMemberHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardSearchDto>> Handle(SearchBoardsRangeForMemberQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.SearchBoardsRangeForMemberAsync(request.UserId, request.SearchTerm, request.Page, request.PageSize, cancellationToken);
            return boards.OrderBy(b => b.Name);
        }, cancellationToken);
    }
}
