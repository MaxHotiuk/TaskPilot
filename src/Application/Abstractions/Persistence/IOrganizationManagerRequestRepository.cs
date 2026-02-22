using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions.Persistence;

public interface IOrganizationManagerRequestRepository : IRepository<OrganizationManagerRequest, Guid>
{
    Task<OrganizationManagerRequest?> GetLatestRequestByUserAndOrganizationAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationManagerRequest>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationManagerRequest>> GetRequestsByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrganizationManagerRequest>> GetRequestsByStatusAsync(ManagerRequestStatus status, CancellationToken cancellationToken = default);
    Task<bool> HasPendingOrRecentRequestAsync(Guid userId, Guid organizationId, int daysThreshold, CancellationToken cancellationToken = default);
}
