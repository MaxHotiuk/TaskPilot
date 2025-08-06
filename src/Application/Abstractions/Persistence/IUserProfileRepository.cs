using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUserProfileRepository : IRepository<UserProfile, Guid>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserProfile>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
}