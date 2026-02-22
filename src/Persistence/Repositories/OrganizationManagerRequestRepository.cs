using Application.Abstractions.Persistence;
using Database;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class OrganizationManagerRequestRepository : Repository<OrganizationManagerRequest, Guid>, IOrganizationManagerRequestRepository
{
    public OrganizationManagerRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<OrganizationManagerRequest?> GetLatestRequestByUserAndOrganizationAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.UserId == userId && r.OrganizationId == organizationId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrganizationManagerRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Status == ManagerRequestStatus.Pending)
            .Include(r => r.User)
            .Include(r => r.Organization)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrganizationManagerRequest>> GetRequestsByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.OrganizationId == organizationId)
            .Include(r => r.User)
            .Include(r => r.Reviewer)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrganizationManagerRequest>> GetRequestsByStatusAsync(ManagerRequestStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Status == status)
            .Include(r => r.User)
            .Include(r => r.Organization)
            .Include(r => r.Reviewer)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingOrRecentRequestAsync(Guid userId, Guid organizationId, int daysThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        
        return await DbSet.AnyAsync(
            r => r.UserId == userId 
                && r.OrganizationId == organizationId
                && (r.Status == ManagerRequestStatus.Pending || r.CreatedAt >= thresholdDate),
            cancellationToken);
    }
}
