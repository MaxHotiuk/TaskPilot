using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Tasks;

public class DeleteTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<DeleteTaskItemCommand>
{
    public DeleteTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var taskItem = await unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            
            if (taskItem is null)
            {
                throw new NotFoundException($"Task with ID {request.Id} was not found");
            }

            unitOfWork.Tasks.Remove(taskItem);
        }, cancellationToken);
    }
}
