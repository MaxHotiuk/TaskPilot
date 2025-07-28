using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IMeetingMemberRepository : IRepository<MeetingMember, (Guid MeetingId, Guid UserId)>
{
}
