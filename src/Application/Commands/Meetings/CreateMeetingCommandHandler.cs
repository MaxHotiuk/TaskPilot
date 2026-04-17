using Application.Abstractions.Messaging;
using Application.Abstractions.Meetings;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Dtos.Meetings;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Meetings;

public class CreateMeetingCommandHandler : BaseCommandHandler, IRequestHandler<CreateMeetingCommand, string>
{
    private readonly IBoardNotifier _boardNotifier;
    private readonly IDailyRoomService _dailyRoomService;
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public CreateMeetingCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IDailyRoomService dailyRoomService, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _dailyRoomService = dailyRoomService;
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task<string> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var (createdId, roomLink) = await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var dto = request.Dto;

            if (dto.ScheduledAt.HasValue && dto.Duration.HasValue)
            {
                var requestStart = dto.ScheduledAt.Value;
                var requestEnd = requestStart.AddMinutes(dto.Duration.Value);

                var existingMeetings = await unitOfWork.Meetings.FindAsync(
                    m => m.CreatedBy == dto.CreatedBy && m.ScheduledAt.HasValue && m.Duration.HasValue,
                    cancellationToken);

                var hasConflict = existingMeetings.Any(m =>
                    m.ScheduledAt!.Value < requestEnd &&
                    m.ScheduledAt!.Value.AddMinutes(m.Duration!.Value) > requestStart);

                if (hasConflict)
                {
                    throw new ValidationException("You already have another meeting scheduled during this time period.");
                }
            }

            var id = Guid.NewGuid();
            var roomUrl = !string.IsNullOrEmpty(dto.ExternalUrl)
                ? dto.ExternalUrl
                : await _dailyRoomService.CreateRoomAsync(id, cancellationToken);

            var meeting = new Meeting
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                Link = roomUrl,
                BoardId = dto.BoardId,
                CreatedBy = dto.CreatedBy,
                ScheduledAt = dto.ScheduledAt,
                Duration = dto.Duration,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Meetings.AddAsync(meeting, cancellationToken);

            foreach (var userId in dto.MemberIds.Distinct())
            {
                var member = new MeetingMember
                {
                    MeetingId = meeting.Id,
                    UserId = userId,
                    Status = "Invited",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await unitOfWork.MeetingMembers.AddAsync(member, cancellationToken);
            }

            await _boardNotifier.NotifyBoardUpdatedAsync(
                meeting.BoardId.ToString(),
                new { action = "meeting_created", meetingId = meeting.Id, meetingTitle = meeting.Title }
            );

            return (meeting.Id, meeting.Link);
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueSyncMeeting(createdId);
        return roomLink;
    }
}
