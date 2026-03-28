using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions.Persistence;

public interface IBoardInvitationRepository : IRepository<BoardInvitation, Guid>
{
    Task<IEnumerable<BoardInvitation>> GetPendingInvitationsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BoardInvitation?> GetPendingInvitationAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingInvitationAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default);
}
