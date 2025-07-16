using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Boards;

public class GetArchivedBoardsByOwnerQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivedBoardsByOwnerQuery, IEnumerable<BoardDto>>
{
    public GetArchivedBoardsByOwnerQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetArchivedBoardsByOwnerQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetArchivedBoardsByOwnerAsync(request.Id, cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
