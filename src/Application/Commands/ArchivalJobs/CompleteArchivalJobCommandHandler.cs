using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class CompleteArchivalJobCommandHandler : IRequestHandler<CompleteArchivalJobCommand, bool>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public CompleteArchivalJobCommandHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<bool> Handle(CompleteArchivalJobCommand request, CancellationToken cancellationToken)
    {
        return await _archivalJobRepository.CompleteJobAsync(
            request.JobId,
            request.BlobPath,
            request.ProcessedBy,
            cancellationToken);
    }
}
