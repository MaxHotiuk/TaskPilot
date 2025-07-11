using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<IEnumerable<Board>> GetBoardsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithStatesAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithTasksAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithMembersAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> SearchBoardsRangeForOwnerAsync(Guid ownerId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> SearchBoardsRangeForUserAsync(Guid userId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}
