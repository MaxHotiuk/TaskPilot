using Application.Abstractions.Persistence;
using Database;
using Domain.Dtos.Chats;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ChatRepository : Repository<Chat, Guid>, IChatRepository
{
    public ChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Chat?> GetChatWithMembersAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(chat => chat.Members)
                .ThenInclude(member => member.User)
            .FirstOrDefaultAsync(chat => chat.Id == chatId, cancellationToken);
    }

    public async Task<Chat?> GetPrivateChatAsync(Guid organizationId, Guid firstUserId, Guid secondUserId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(chat => chat.Members)
            .FirstOrDefaultAsync(chat =>
                chat.OrganizationId == organizationId &&
                chat.Type == ChatType.Private &&
                chat.Members.Count == 2 &&
                chat.Members.Any(member => member.UserId == firstUserId) &&
                chat.Members.Any(member => member.UserId == secondUserId),
                cancellationToken);
    }

    public async Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId, Guid? organizationId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(chat => chat.Members.Any(member => member.UserId == userId));

        if (organizationId.HasValue)
        {
            query = query.Where(chat => chat.OrganizationId == organizationId.Value);
        }

        return await query
            .OrderByDescending(chat => chat.Messages.OrderByDescending(message => message.CreatedAt)
                .Select(message => message.CreatedAt)
                .FirstOrDefault())
            .Select(chat => new ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = chat.Messages
                    .OrderByDescending(message => message.CreatedAt)
                    .Select(message => new ChatMessagePreviewDto
                    {
                        Id = message.Id,
                        SenderId = message.SenderId,
                        Content = message.Content,
                        CreatedAt = message.CreatedAt
                    })
                    .FirstOrDefault(),
                Members = chat.Members
                    .Select(member => new ChatMemberDto
                    {
                        UserId = member.UserId,
                        Username = member.User.Username,
                        Email = member.User.Email,
                        Role = member.Role,
                        LastReadAt = member.LastReadAt
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
