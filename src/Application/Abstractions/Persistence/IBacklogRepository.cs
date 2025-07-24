using Domain.Dtos.Backlog;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBacklogRepository : IRepository<Backlog, Guid>
{
    Task<IEnumerable<BacklogDto>> SearchBacklogsForBoardAsync(Guid boardId, string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}
