using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ICommentRepository : IRepository<Comment, Guid>
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> SearchCommentsRangeByTaskIdAsync(string searchTerm, Guid taskid, int page, int pageSize, CancellationToken cancellationToken = default);
}
