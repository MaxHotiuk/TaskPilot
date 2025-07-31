using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.Meetings;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Meetings;

public class CreateMeetingCommandHandler : BaseCommandHandler, IRequestHandler<CreateMeetingCommand, string>
{
    private readonly IBoardNotifier _boardNotifier;

    public CreateMeetingCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task<string> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var dto = request.Dto;
            var id = Guid.NewGuid();

            var meeting = new Meeting
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                Link = dto.Domain + $"/board/{dto.BoardId}/meeting/{id}",
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

            return meeting.Link;
        }, cancellationToken);
    }
}
