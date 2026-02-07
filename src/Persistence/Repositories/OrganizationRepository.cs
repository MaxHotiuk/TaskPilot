using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;

namespace Persistence.Repositories;

public class OrganizationRepository : Repository<Organization, Guid>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Organization?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(o => o.Domain == domain, cancellationToken);
    }

    public async Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(o => o.Domain == domain, cancellationToken);
    }

    public async Task<IEnumerable<Organization>> GetOrganizationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.OrganizationMembers
            .Where(om => om.UserId == userId)
            .Include(om => om.Organization)
            .Select(om => om.Organization)
            .ToListAsync(cancellationToken);
    }
}
