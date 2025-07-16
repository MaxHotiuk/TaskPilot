using Application.Common.Dtos.Boards;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task<IEnumerable<Board>> GetBoardsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithStatesAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithTasksAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithMembersAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForOwnerAsync(Guid ownerId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForUserAsync(Guid userId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForMemberAsync(Guid memberId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetArchivedBoardsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetArchivedBoardsForProcessingAsync(DateTime? processedBefore = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetArchivedBoardsPendingJobsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetArchivedBoardsByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<Board?> GetBoardWithArchivalJobsAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<bool> MarkBoardAsArchivedAsync(Guid boardId, string? archivalReason = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateBoardArchivalStatusAsync(Guid boardId, bool isArchived, DateTime? archivedAt = null, string? archivalReason = null, CancellationToken cancellationToken = default);
}
