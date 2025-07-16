using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsForProcessingQueryHandler : IRequestHandler<GetArchivalJobsForProcessingQuery, IEnumerable<ArchivalJobDto>>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public GetArchivalJobsForProcessingQueryHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsForProcessingQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _archivalJobRepository.GetJobsForProcessingAsync(request.CreatedBefore, cancellationToken);
        return jobs.ToDto();
    }
}
