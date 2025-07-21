using Application.Abstractions.Persistence;

namespace Application.Common.Handlers;

public abstract class BaseCommandHandler
{
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

    protected BaseCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        UnitOfWorkFactory = unitOfWorkFactory;
    }

    protected async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<IUnitOfWork, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        using var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken);

        try
        {
            var result = await operation(unitOfWork);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    protected async Task ExecuteInTransactionAsync(
        Func<IUnitOfWork, Task> operation,
        CancellationToken cancellationToken = default)
    {
        using var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken);

        try
        {
            await operation(unitOfWork);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
