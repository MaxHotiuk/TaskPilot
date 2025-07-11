using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Domain.Entities;
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
            .Where(b => b.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Board>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.OwnerId == userId || b.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Board?> GetBoardWithStatesAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.States.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }

    public async Task<Board?> GetBoardWithTasksAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Tasks)
                .ThenInclude(t => t.State)
            .Include(b => b.Tasks)
                .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }

    public async Task<Board?> GetBoardWithMembersAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Members)
                .ThenInclude(m => m.User)
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);
    }

    public async Task<IEnumerable<BoardSearchDto>> SearchBoardsRangeForOwnerAsync(Guid ownerId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.OwnerId == ownerId && b.Name.Contains(searchTerm))
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
            .Where(b => (b.OwnerId == userId || b.Members.Any(m => m.UserId == userId)) && b.Name.Contains(searchTerm))
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
            .Where(b => b.Members.Any(m => m.UserId == memberId) && b.Name.Contains(searchTerm))
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
}
