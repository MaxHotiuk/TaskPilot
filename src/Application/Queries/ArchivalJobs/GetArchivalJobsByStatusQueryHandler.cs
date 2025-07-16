using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsByStatusQueryHandler : IRequestHandler<GetArchivalJobsByStatusQuery, IEnumerable<ArchivalJobDto>>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public GetArchivalJobsByStatusQueryHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsByStatusQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _archivalJobRepository.GetJobsByStatusAsync((Domain.Entities.ArchivalStatus)request.Status, cancellationToken);
        return jobs.ToDto();
    }
}
