using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class UpdateArchivalJobStatusCommandHandler : BaseCommandHandler, IRequestHandler<UpdateArchivalJobStatusCommand, bool>
{
    public UpdateArchivalJobStatusCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(UpdateArchivalJobStatusCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.ArchivalJobs.UpdateJobStatusAsync(
                request.JobId,
                (Domain.Entities.ArchivalStatus)request.Status,
                request.ErrorMessage,
                request.ProcessedBy,
                cancellationToken);
        }, cancellationToken);
    }
}
