using Application.Abstractions.Persistence;
using Domain.Dtos.Backlog;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BacklogRepository : Repository<Backlog, Guid>, IBacklogRepository
{
    private readonly ApplicationDbContext _context;

    public BacklogRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BacklogDto>> SearchBacklogsForBoardAsync(
        Guid boardId,
        string searchTerm,
        int page,
        int pageSize,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Backlog
            .Where(b => b.BoardId == boardId &&
                        (string.IsNullOrEmpty(searchTerm) || b.Description.Contains(searchTerm)) &&
                        b.CreatedAt.Date >= startDate.ToDateTime(TimeOnly.MinValue) &&
                        b.CreatedAt.Date <= endDate.ToDateTime(TimeOnly.MinValue))
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BacklogDto
            {
                Description = b.Description,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
