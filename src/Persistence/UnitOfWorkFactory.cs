using Application.Abstractions.Persistence;
using Database;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Repositories;

namespace Persistence;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly ApplicationDbContext _context;

    public UnitOfWorkFactory(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new UnitOfWork(_context, transaction);
    }
}
