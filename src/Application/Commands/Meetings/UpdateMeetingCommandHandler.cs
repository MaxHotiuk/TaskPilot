using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Meetings;

public class UpdateMeetingCommandHandler : BaseCommandHandler, IRequestHandler<UpdateMeetingCommand>
{
    private readonly IBoardNotifier _boardNotifier;
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public UpdateMeetingCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var meeting = await unitOfWork.Meetings.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Meeting with ID {request.Id} was not found");

            meeting.Title = request.Title;
            meeting.Description = request.Description;
            meeting.ScheduledAt = request.ScheduledAt;
            meeting.Duration = request.Duration;
            meeting.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Meetings.Update(meeting);

            await _boardNotifier.NotifyBoardUpdatedAsync(
                meeting.BoardId.ToString(),
                new { action = "meeting_updated", meetingId = meeting.Id, meetingTitle = meeting.Title }
            );
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueSyncMeeting(request.Id);
    }
}
