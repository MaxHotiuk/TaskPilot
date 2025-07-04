using Application.Abstractions.Persistence;

namespace Application.Common.Handlers;

public abstract class BaseQueryHandler
{
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

    protected BaseQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        UnitOfWorkFactory = unitOfWorkFactory;
    }

    protected async Task<TResult> ExecuteQueryAsync<TResult>(
        Func<IUnitOfWork, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        using var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken);
        return await operation(unitOfWork);
    }
}
