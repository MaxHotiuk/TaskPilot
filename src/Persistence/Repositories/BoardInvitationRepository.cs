using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BoardInvitationRepository : Repository<BoardInvitation, Guid>, IBoardInvitationRepository
{
    public BoardInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BoardInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardInvitations
            .Include(bi => bi.Board)
            .Include(bi => bi.Inviter)
            .Where(bi => bi.UserId == userId && bi.Status == InvitationStatus.Pending)
            .OrderByDescending(bi => bi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<BoardInvitation?> GetPendingInvitationAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardInvitations
            .Include(bi => bi.Board)
            .Include(bi => bi.Inviter)
            .FirstOrDefaultAsync(bi => bi.BoardId == boardId && bi.UserId == userId && bi.Status == InvitationStatus.Pending, cancellationToken);
    }

    public async Task<bool> HasPendingInvitationAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.BoardInvitations
            .AnyAsync(bi => bi.BoardId == boardId && bi.UserId == userId && bi.Status == InvitationStatus.Pending, cancellationToken);
    }
}
