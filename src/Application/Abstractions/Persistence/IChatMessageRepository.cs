using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IChatMessageRepository : IRepository<ChatMessage, Guid>
{
    Task<IEnumerable<ChatMessage>> GetMessagesByChatIdAsync(Guid chatId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ChatMessage?> GetLatestMessageAsync(Guid chatId, CancellationToken cancellationToken = default);
}
