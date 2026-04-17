
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Meetings;

public class DeleteMeetingCommandHandler : BaseCommandHandler, IRequestHandler<DeleteMeetingCommand>
{
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public DeleteMeetingCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var meeting = await unitOfWork.Meetings.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Meeting with ID {request.Id} was not found");
            unitOfWork.Meetings.Remove(meeting);
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueDeleteMeeting(request.Id);
    }
}
