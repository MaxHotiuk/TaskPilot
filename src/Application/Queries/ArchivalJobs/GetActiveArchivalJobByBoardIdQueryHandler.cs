using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetActiveArchivalJobByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetActiveArchivalJobByBoardIdQuery, ArchivalJobDto?>
{
    public GetActiveArchivalJobByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<ArchivalJobDto?> Handle(GetActiveArchivalJobByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var job = await unitOfWork.ArchivalJobs.GetActiveJobByBoardIdAsync(request.BoardId, cancellationToken);
            return job?.ToDto();
        }, cancellationToken);
    }
}
