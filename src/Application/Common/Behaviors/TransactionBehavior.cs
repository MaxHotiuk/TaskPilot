using Application.Abstractions.Persistence;
using MediatR;

namespace Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public TransactionBehavior(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        
        try
        {
            var response = await next();
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            
            return response;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}
