using Application.Abstractions.Persistence;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ChatMessageRepository : Repository<ChatMessage, Guid>, IChatMessageRepository
{
    public ChatMessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var messages = await DbSet
            .Where(message => message.ChatId == chatId)
            .OrderByDescending(message => message.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(message => message.Sender)
            .ToListAsync(cancellationToken);

        return messages.OrderBy(message => message.CreatedAt);
    }

    public async Task<ChatMessage?> GetLatestMessageAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(message => message.ChatId == chatId)
            .OrderByDescending(message => message.CreatedAt)
            .Include(message => message.Sender)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
