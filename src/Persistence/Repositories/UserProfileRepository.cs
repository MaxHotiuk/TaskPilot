using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;

namespace Persistence.Repositories;

public class UserProfileRepository : Repository<UserProfile, Guid>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(up => up.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(up => up.Department == department).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(up => up.Location == location).ToListAsync(cancellationToken);
    }
}