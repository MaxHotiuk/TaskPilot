using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ITagRepository : IRepository<Tag, int>
{
    Task<IEnumerable<Tag>> GetTagsByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Tag?> GetTagByBoardAndNameAsync(Guid boardId, string name, CancellationToken cancellationToken = default);
    Task<bool> IsValidTagForBoardAsync(int tagId, Guid boardId, CancellationToken cancellationToken = default);
}
