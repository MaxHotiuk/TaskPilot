using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class FailArchivalJobCommandHandler : IRequestHandler<FailArchivalJobCommand, bool>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public FailArchivalJobCommandHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<bool> Handle(FailArchivalJobCommand request, CancellationToken cancellationToken)
    {
        return await _archivalJobRepository.FailJobAsync(
            request.JobId,
            request.ErrorMessage,
            request.ProcessedBy,
            cancellationToken);
    }
}
