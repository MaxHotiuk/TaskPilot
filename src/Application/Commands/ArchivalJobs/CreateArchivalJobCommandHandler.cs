using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ArchivalJobs;

public class CreateArchivalJobCommandHandler : BaseCommandHandler, IRequestHandler<CreateArchivalJobCommand, ArchivalJobDto>
{
    public CreateArchivalJobCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<ArchivalJobDto> Handle(CreateArchivalJobCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var job = await unitOfWork.ArchivalJobs.CreateJobAsync(
                request.BoardId,
                request.JobType,
                request.Metadata,
                cancellationToken);
            return job.ToDto();
        }, cancellationToken);
    }
}
