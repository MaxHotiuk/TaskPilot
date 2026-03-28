using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class OrganizationInvitationRepository : Repository<OrganizationInvitation, Guid>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OrganizationInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.OrganizationInvitations
            .Include(oi => oi.Organization)
            .Include(oi => oi.Inviter)
            .Where(oi => oi.UserId == userId && oi.Status == InvitationStatus.Pending)
            .OrderByDescending(oi => oi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrganizationInvitation?> GetPendingInvitationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.OrganizationInvitations
            .Include(oi => oi.Organization)
            .Include(oi => oi.Inviter)
            .FirstOrDefaultAsync(oi => oi.OrganizationId == organizationId && oi.UserId == userId && oi.Status == InvitationStatus.Pending, cancellationToken);
    }

    public async Task<bool> HasPendingInvitationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.OrganizationInvitations
            .AnyAsync(oi => oi.OrganizationId == organizationId && oi.UserId == userId && oi.Status == InvitationStatus.Pending, cancellationToken);
    }
}
