using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MeetingMemberRepository : Repository<MeetingMember, (Guid MeetingId, Guid UserId)>, IMeetingMemberRepository
{
    public MeetingMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MeetingMember>> GetMembersByMeetingIdAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        var members = await DbSet
            .Where(mm => mm.MeetingId == meetingId)
            .Include(mm => mm.Meeting)
            .Include(mm => mm.User)
            .ToListAsync(cancellationToken);

        var meeting = members.FirstOrDefault()?.Meeting;
        if (meeting == null)
        {
            return members;
        }

        bool ownerIsMember = members.Any(m => m.UserId == meeting.CreatedBy);
        if (!ownerIsMember)
        {
            var owner = new MeetingMember
            {
                MeetingId = meeting.Id,
                UserId = meeting.CreatedBy,
                Status = "Owner",
                Meeting = meeting,
                User = meeting.Creator
            };
            members.Insert(0, owner);
        }

        return members;
    }
}
