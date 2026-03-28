using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;

namespace Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEntraIdAsync(string entraId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.EntraId == entraId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEntraIdAsync(string entraId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.EntraId == entraId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default)
    {
        var userIds = await Context.OrganizationMembers
            .Where(om => organizationIds.Contains(om.OrganizationId))
            .Select(om => om.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await DbSet
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }
}
