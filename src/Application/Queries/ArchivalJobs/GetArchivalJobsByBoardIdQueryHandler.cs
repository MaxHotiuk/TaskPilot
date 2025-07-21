using Application.Abstractions.Persistence;
using Domain.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetArchivalJobsByBoardIdQueryHandler : IRequestHandler<GetArchivalJobsByBoardIdQuery, IEnumerable<ArchivalJobDto>>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public GetArchivalJobsByBoardIdQueryHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<IEnumerable<ArchivalJobDto>> Handle(GetArchivalJobsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _archivalJobRepository.GetJobsByBoardIdAsync(request.BoardId, cancellationToken);
        return jobs.ToDto();
    }
}
