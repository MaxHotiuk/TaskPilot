using Application.Abstractions.Persistence;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class OrganizationMemberRepository : Repository<OrganizationMember, object>, IOrganizationMemberRepository
{
    public OrganizationMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsMemberOfOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(om => om.OrganizationId == organizationId && om.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<OrganizationMember>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(om => om.UserId == userId)
            .Include(om => om.Organization)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(om => om.OrganizationId == organizationId)
            .Include(om => om.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetOrganizationIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(om => om.UserId == userId)
            .Select(om => om.OrganizationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrganizationMember?> GetOrganizationMemberAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(om => om.OrganizationId == organizationId && om.UserId == userId, cancellationToken);
    }
}
