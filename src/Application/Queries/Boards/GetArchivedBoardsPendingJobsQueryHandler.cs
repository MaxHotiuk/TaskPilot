using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Boards;

public class GetArchivedBoardsPendingJobsQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivedBoardsPendingJobsQuery, IEnumerable<BoardDto>>
{
    public GetArchivedBoardsPendingJobsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetArchivedBoardsPendingJobsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetArchivedBoardsPendingJobsAsync(cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
