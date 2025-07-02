using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BoardMemberRepository : Repository<BoardMember, object>, IBoardMemberRepository
{
    public BoardMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BoardMember>> GetMembersByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardMembers
            .Where(bm => bm.BoardId == boardId)
            .Include(bm => bm.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BoardMember>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardMembers
            .Where(bm => bm.UserId == userId)
            .Include(bm => bm.Board)
            .ToListAsync(cancellationToken);
    }

    public async Task<BoardMember?> GetBoardMemberAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardMembers
            .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsMemberOfBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardMembers
            .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId, cancellationToken);
    }
}
