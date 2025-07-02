using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBoardMemberRepository : IRepository<BoardMember, object>
{
    Task<IEnumerable<BoardMember>> GetMembersByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BoardMember>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BoardMember?> GetBoardMemberAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberOfBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default);
}
