using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MeetingMemberRepository : Repository<MeetingMember, (Guid MeetingId, Guid UserId)>, IMeetingMemberRepository
{
    public MeetingMemberRepository(ApplicationDbContext context) : base(context)
    {
    }
}
