using System.Threading;
using System.Threading.Tasks;
using Application.Commands.ArchivalJobs;
using MediatR;
using Application.Abstractions.Archivation;
using Application.Common.Dtos.Boards;
using Application.Abstractions.Persistence;

namespace Application.Commands.ArchivalJobs
{
    public class RemoveArchivalJobCommandHandler : IRequestHandler<RemoveArchivalJobCommand, bool>
    {
        private readonly ICosmosArchivalJobRepository _archivalJobRepository;

        public RemoveArchivalJobCommandHandler(ICosmosArchivalJobRepository archivalJobRepository)
        {
            _archivalJobRepository = archivalJobRepository;
        }

        public async Task<bool> Handle(RemoveArchivalJobCommand request, CancellationToken cancellationToken)
        {
            return await _archivalJobRepository.RemoveJobAsync(request.JobId, cancellationToken);
        }
    }
}
