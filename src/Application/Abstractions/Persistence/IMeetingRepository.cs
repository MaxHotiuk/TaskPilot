using Domain.Dtos.Meetings;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IMeetingRepository : IRepository<Meeting, Guid>
{
    Task<IEnumerable<Meeting>> GetMeetingsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Meeting>> GetMeetingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MeetingCalendarItemDto>> GetMeetingCalendarItemsAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
