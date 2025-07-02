using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class StateRepository : Repository<State, int>, IStateRepository
{
    public StateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<State>> GetStatesByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.BoardId == boardId)
            .OrderBy(s => s.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<State?> GetStateByBoardAndNameAsync(Guid boardId, string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Name == name, cancellationToken);
    }

    public async Task<bool> IsValidStateForBoardAsync(int stateId, Guid boardId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(s => s.Id == stateId && s.BoardId == boardId, cancellationToken);
    }
}
