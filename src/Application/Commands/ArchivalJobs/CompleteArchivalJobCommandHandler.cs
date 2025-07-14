using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class CompleteArchivalJobCommandHandler : BaseCommandHandler, IRequestHandler<CompleteArchivalJobCommand, bool>
{
    public CompleteArchivalJobCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(CompleteArchivalJobCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.ArchivalJobs.CompleteJobAsync(
                request.JobId,
                request.BlobPath,
                request.ProcessedBy,
                cancellationToken);
        }, cancellationToken);
    }
}
