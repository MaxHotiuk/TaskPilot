using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions.Persistence;

public interface IOrganizationInvitationRepository : IRepository<OrganizationInvitation, Guid>
{
    Task<IEnumerable<OrganizationInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrganizationInvitation?> GetPendingInvitationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingInvitationAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
}
