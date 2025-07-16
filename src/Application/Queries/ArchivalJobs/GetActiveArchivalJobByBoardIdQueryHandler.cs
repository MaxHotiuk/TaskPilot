using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.ArchivalJobs;

public class GetActiveArchivalJobByBoardIdQueryHandler : IRequestHandler<GetActiveArchivalJobByBoardIdQuery, ArchivalJobDto?>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public GetActiveArchivalJobByBoardIdQueryHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<ArchivalJobDto?> Handle(GetActiveArchivalJobByBoardIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _archivalJobRepository.GetActiveJobByBoardIdAsync(request.BoardId, cancellationToken);
        return job?.ToDto();
    }
}
