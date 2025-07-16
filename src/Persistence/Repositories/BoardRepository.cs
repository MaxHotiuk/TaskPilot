using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BoardRepository : Repository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Board>> GetBoardsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.OwnerId == ownerId && !b.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)) && !b.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<Board?> GetBoardWithStatesAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.States.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(b => b.Id == boardId && !b.IsArchived, cancellationToken);
    }

    public async Task<Board?> GetBoardWithTasksAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Tasks)
                .ThenInclude(t => t.State)
            .Include(b => b.Tasks)
                .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(b => b.Id == boardId && !b.IsArchived, cancellationToken);
    }

    public async Task<Board?> GetBoardWithMembersAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Members)
                .ThenInclude(m => m.User)
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == boardId && !b.IsArchived, cancellationToken);
    }

    public async Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForOwnerAsync(Guid ownerId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.OwnerId == ownerId && !b.IsArchived && b.Name.Contains(searchTerm))
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BoardSearchDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                NumberOfMembers = b.Members.Count,
                NumberOfTasks = b.Tasks.Count,
                OwnerId = b.OwnerId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForUserAsync(Guid userId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)) && !b.IsArchived && b.Name.Contains(searchTerm))
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BoardSearchDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                NumberOfMembers = b.Members.Count,
                NumberOfTasks = b.Tasks.Count,
                OwnerId = b.OwnerId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForMemberAsync(Guid memberId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.Members.Any(m => m.UserId == memberId) && !b.IsArchived && b.Name.Contains(searchTerm))
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BoardSearchDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                NumberOfMembers = b.Members.Count,
                NumberOfTasks = b.Tasks.Count,
                OwnerId = b.OwnerId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetArchivedBoardsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetArchivedBoardsForProcessingAsync(DateTime? processedBefore = null, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.IsArchived && (processedBefore == null || b.ArchivedAt < processedBefore))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetArchivedBoardsPendingJobsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.IsArchived && b.ArchivalJobs.Any(j => j.Status == ArchivalStatus.Pending))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetArchivedBoardsByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.IsArchived && b.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Board?> GetBoardWithArchivalJobsAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.ArchivalJobs)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }

    public async Task<bool> MarkBoardAsArchivedAsync(Guid boardId, string? archivalReason = null, CancellationToken cancellationToken = default)
    {
        var board = await GetByIdAsync(boardId, cancellationToken);
        if (board == null) return false;

        board.IsArchived = true;
        board.ArchivedAt = DateTime.UtcNow;
        board.ArchivalReason = archivalReason;

        Context.Boards.Update(board);
        return await Context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateBoardArchivalStatusAsync(Guid boardId, bool isArchived, DateTime? archivedAt = null, string? archivalReason = null, CancellationToken cancellationToken = default)
    {
        var board = await GetByIdAsync(boardId, cancellationToken);
        if (board == null) return false;

        board.IsArchived = isArchived;
        board.ArchivedAt = archivedAt ?? (isArchived ? DateTime.UtcNow : null);
        board.ArchivalReason = archivalReason;

        Context.Boards.Update(board);
        return await Context.SaveChangesAsync(cancellationToken) > 0;
    }
}
