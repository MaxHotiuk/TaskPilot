using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEntraIdAsync(string entraId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEntraIdAsync(string entraId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default);
}
