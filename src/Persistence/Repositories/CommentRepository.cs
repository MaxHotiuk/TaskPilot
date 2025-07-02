using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class CommentRepository : Repository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await Context.Comments
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .Include(c => c.Author)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await Context.Comments
            .Where(c => c.AuthorId == authorId)
            .OrderByDescending(c => c.CreatedAt)
            .Include(c => c.Task)
            .ToListAsync(cancellationToken);
    }
}
