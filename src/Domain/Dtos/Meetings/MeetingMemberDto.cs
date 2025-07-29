using System;

namespace Domain.Dtos.Meetings;

public record MeetingMemberDto
{
    public Guid MeetingId { get; init; }
    public Guid UserId { get; init; }
    public string Status { get; init; } = string.Empty;
}
