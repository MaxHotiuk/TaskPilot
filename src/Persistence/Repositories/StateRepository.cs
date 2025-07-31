using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Database;

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

    public async Task SwapStateOrderAsync(int firstStateId, int secondStateId, Guid boardId, CancellationToken cancellationToken = default)
    {
        var states = await DbSet
            .Where(s => (s.Id == firstStateId || s.Id == secondStateId) && s.BoardId == boardId)
            .Select(s => new { s.Id, s.Order })
            .ToListAsync(cancellationToken);

        var firstStateOrder = states.First(s => s.Id == firstStateId).Order;
        var secondStateOrder = states.First(s => s.Id == secondStateId).Order;

        await Context.Database.ExecuteSqlRawAsync(
            "EXEC UpdateStatesOrder @Id1 = {0}, @Order1 = {1}, @Id2 = {2}, @Order2 = {3}, @BoardId = {4}",
            firstStateId, secondStateOrder, secondStateId, firstStateOrder, boardId);
    }
}
