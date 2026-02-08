using Application.Abstractions.Persistence;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ChatMemberRepository : Repository<ChatMember, object>, IChatMemberRepository
{
    public ChatMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(member => member.ChatId == chatId && member.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetMemberIdsAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(member => member.ChatId == chatId)
            .Select(member => member.UserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatMember>> GetMembersAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(member => member.ChatId == chatId)
            .Include(member => member.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(member => member.ChatId == chatId && member.UserId == userId, cancellationToken);
    }
}
