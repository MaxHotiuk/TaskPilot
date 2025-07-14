using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class FailArchivalJobCommandHandler : BaseCommandHandler, IRequestHandler<FailArchivalJobCommand, bool>
{
    public FailArchivalJobCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(FailArchivalJobCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.ArchivalJobs.FailJobAsync(
                request.JobId,
                request.ErrorMessage,
                request.ProcessedBy,
                cancellationToken);
        }, cancellationToken);
    }
}
