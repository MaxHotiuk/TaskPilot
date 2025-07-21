using Application.Abstractions.Persistence;
using Domain.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Boards;

public class GetArchivedBoardsForProcessingQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivedBoardsForProcessingQuery, IEnumerable<BoardDto>>
{
    public GetArchivedBoardsForProcessingQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetArchivedBoardsForProcessingQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetArchivedBoardsForProcessingAsync(request.ProcessedBefore, cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
