using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember, object>
{
    Task<bool> IsMemberOfOrganizationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationMember>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetOrganizationIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrganizationMember?> GetOrganizationMemberAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
}
