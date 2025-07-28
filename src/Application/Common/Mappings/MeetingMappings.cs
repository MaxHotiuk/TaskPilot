using Domain.Dtos.Meetings;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class MeetingMappings
{
    public static MeetingDto ToDto(this Meeting meeting)
    {
        return new MeetingDto
        {
            Id = meeting.Id,
            Title = meeting.Title,
            Description = meeting.Description ?? string.Empty,
            Link = meeting.Link ?? string.Empty,
            BoardId = meeting.BoardId,
            CreatedBy = meeting.CreatedBy,
            ScheduledAt = meeting.ScheduledAt,
            Duration = meeting.Duration,
            Status = meeting.Status
        };
    }
}
