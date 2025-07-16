using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class CreateArchivalJobCommandHandler : IRequestHandler<CreateArchivalJobCommand, ArchivalJobDto>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public CreateArchivalJobCommandHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<ArchivalJobDto> Handle(CreateArchivalJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _archivalJobRepository.CreateJobAsync(
            request.BoardId,
            request.JobType,
            request.Metadata,
            cancellationToken);
        return job.ToDto();
    }
}
