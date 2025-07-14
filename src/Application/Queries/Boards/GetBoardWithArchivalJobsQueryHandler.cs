using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Boards;

public class GetBoardWithArchivalJobsQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardWithArchivalJobsQuery, BoardDto?>
{
    public GetBoardWithArchivalJobsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<BoardDto?> Handle(GetBoardWithArchivalJobsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetBoardWithArchivalJobsAsync(request.BoardId, cancellationToken);
            return board?.ToDto();
        }, cancellationToken);
    }
}
