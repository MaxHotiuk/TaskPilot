using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetArchivalJobsByBoardIdQuery, IEnumerable<ArchivalJobDto>>
{
    public GetArchivalJobsByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var jobs = await unitOfWork.ArchivalJobs.GetJobsByBoardIdAsync(request.BoardId, cancellationToken);
            return jobs.ToDto();
        }, cancellationToken);
    }
}
