using Domain.Dtos.Meetings;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class MeetingMemberMappings
{
    public static MeetingMemberDto ToDto(this MeetingMember member)
    {
        return new MeetingMemberDto
        {
            MeetingId = member.MeetingId,
            UserId = member.UserId,
            Status = member.Status
        };
    }
}
