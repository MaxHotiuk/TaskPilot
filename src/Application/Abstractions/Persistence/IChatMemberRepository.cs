using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IChatMemberRepository : IRepository<ChatMember, object>
{
    Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetMemberIdsAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatMember>> GetMembersAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
}
