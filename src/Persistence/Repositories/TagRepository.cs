using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;

namespace Persistence.Repositories;

public class TagRepository : Repository<Tag, int>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> GetTagsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.BoardId == boardId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetTagByBoardAndNameAsync(Guid boardId, string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t => t.BoardId == boardId && t.Name == name, cancellationToken);
    }

    public async Task<bool> IsValidTagForBoardAsync(int tagId, Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(t => t.Id == tagId && t.BoardId == boardId, cancellationToken);
    }
}
