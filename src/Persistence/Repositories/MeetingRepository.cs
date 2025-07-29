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

    public async Task<IEnumerable<MeetingDto>> GetMeetingsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        var meetings = await DbSet
            .Where(m => m.BoardId == boardId)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description ?? string.Empty,
                Link = m.Link ?? string.Empty,
                BoardId = m.BoardId,
                CreatedBy = m.CreatedBy,
                ScheduledAt = m.ScheduledAt,
                Duration = m.Duration,
                Status = m.Status,
                MemberIds = m.Members.Select(mm => mm.UserId).ToList()
            })
            .ToListAsync(cancellationToken);

        meetings.ForEach(m =>
        {
            if (!m.MemberIds!.Contains(m.CreatedBy))
            {
                m.MemberIds.Add(m.CreatedBy);
            }
        });

        return meetings;
    }


    public async Task<IEnumerable<MeetingDto>> GetMeetingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var meetings = await DbSet
            .Where(m => m.Members.Any(mm => mm.UserId == userId) || m.CreatedBy == userId)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description ?? string.Empty,
                Link = m.Link ?? string.Empty,
                BoardId = m.BoardId,
                CreatedBy = m.CreatedBy,
                ScheduledAt = m.ScheduledAt,
                Duration = m.Duration,
                Status = m.Status,
                MemberIds = m.Members.Select(mm => mm.UserId).ToList()
            })
            .ToListAsync(cancellationToken);

        meetings.ForEach(m =>
        {
            if (!m.MemberIds!.Contains(m.CreatedBy))
            {
                m.MemberIds.Add(m.CreatedBy);
            }
        });
        return meetings;
    }

    public async Task<IEnumerable<MeetingCalendarItemDto>> GetMeetingCalendarItemsAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var meetings = await DbSet
            .Where(m =>
            m.ScheduledAt.HasValue &&
            m.ScheduledAt.Value.Date >= startDate.Date &&
            m.ScheduledAt.Value.Date <= endDate.Date &&
            (m.Members.Any(mm => mm.UserId == userId) || m.CreatedBy == userId))
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

        return meetings;
    }
}
