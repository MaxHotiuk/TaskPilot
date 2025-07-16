using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class UpdateArchivalJobStatusCommandHandler : IRequestHandler<UpdateArchivalJobStatusCommand, bool>
{
    private readonly ICosmosArchivalJobRepository _archivalJobRepository;
    public UpdateArchivalJobStatusCommandHandler(ICosmosArchivalJobRepository archivalJobRepository)
    {
        _archivalJobRepository = archivalJobRepository;
    }

    public async Task<bool> Handle(UpdateArchivalJobStatusCommand request, CancellationToken cancellationToken)
    {
        return await _archivalJobRepository.UpdateJobStatusAsync(
            request.JobId,
            (Domain.Entities.ArchivalStatus)request.Status,
            request.ErrorMessage,
            request.ProcessedBy,
            cancellationToken);
    }
}
