using Domain.Dtos.Chats;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IChatRepository : IRepository<Chat, Guid>
{
    Task<Chat?> GetChatWithMembersAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<Chat?> GetPrivateChatAsync(Guid organizationId, Guid firstUserId, Guid secondUserId, CancellationToken cancellationToken = default);
    Task<Chat?> GetBoardChatAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatDto>> GetChatsForUserAsync(Guid userId, Guid? organizationId = null, CancellationToken cancellationToken = default);
}
