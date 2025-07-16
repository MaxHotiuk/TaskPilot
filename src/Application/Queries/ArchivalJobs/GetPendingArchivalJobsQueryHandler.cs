using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetPendingArchivalJobsQueryHandler : IRequestHandler<GetPendingArchivalJobsQuery, IEnumerable<ArchivalJobDto>>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public GetPendingArchivalJobsQueryHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetPendingArchivalJobsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _archivalJobRepository.GetPendingJobsAsync(cancellationToken);
        return jobs.ToDto();
    }
}
