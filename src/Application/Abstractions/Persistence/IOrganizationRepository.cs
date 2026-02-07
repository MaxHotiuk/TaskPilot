using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IOrganizationRepository : IRepository<Organization, Guid>
{
    Task<Organization?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<IEnumerable<Organization>> GetOrganizationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
