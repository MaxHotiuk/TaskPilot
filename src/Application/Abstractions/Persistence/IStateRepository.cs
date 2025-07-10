using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IStateRepository : IRepository<State, int>
{
    Task<IEnumerable<State>> GetStatesByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<State?> GetStateByBoardAndNameAsync(Guid boardId, string name, CancellationToken cancellationToken = default);
    Task<bool> IsValidStateForBoardAsync(int stateId, Guid boardId, CancellationToken cancellationToken = default);
    Task SwapStateOrderAsync(int firstStateId, int secondStateId, Guid boardId, CancellationToken cancellationToken = default);
}
