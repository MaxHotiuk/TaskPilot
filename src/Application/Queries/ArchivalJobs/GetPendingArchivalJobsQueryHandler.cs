using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetPendingArchivalJobsQueryHandler : BaseQueryHandler, IRequestHandler<GetPendingArchivalJobsQuery, IEnumerable<ArchivalJobDto>>
{
    public GetPendingArchivalJobsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetPendingArchivalJobsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var jobs = await unitOfWork.ArchivalJobs.GetPendingJobsAsync(cancellationToken);
            return jobs.ToDto();
        }, cancellationToken);
    }
}
