using Application.Abstractions.Persistence;
using Domain.Dtos.Meetings;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MeetingRepository : Repository<Meeting, Guid>, IMeetingRepository
{
    public MeetingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Meeting>> GetMeetingsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await Context.Meetings
            .Where(m => m.BoardId == boardId)
            .ToListAsync(cancellationToken);
    }


    public async Task<IEnumerable<Meeting>> GetMeetingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.Members.Any(mm => mm.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MeetingCalendarItemDto>> GetMeetingCalendarItemsAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.ScheduledAt >= startDate && m.ScheduledAt <= endDate && m.Members.Any(mm => mm.UserId == userId))
            .Select(m => new MeetingCalendarItemDto
            {
                Id = m.Id,
                BoardId = m.BoardId,
                Title = m.Title,
                ScheduledAt = m.ScheduledAt,
                Duration = m.Duration,
                Description = m.Description
            })
            .ToListAsync(cancellationToken);
    }
}
