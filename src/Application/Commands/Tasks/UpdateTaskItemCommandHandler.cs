using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Tasks;

public class UpdateTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<UpdateTaskItemCommand>
{
    public UpdateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var taskItem = await unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            
            if (taskItem is null)
            {
                throw new NotFoundException($"Task with ID {request.Id} was not found");
            }

            if (!await unitOfWork.States.IsValidStateForBoardAsync(request.StateId, taskItem.BoardId, cancellationToken))
            {
                throw new ValidationException($"State with ID {request.StateId} is not valid for board {taskItem.BoardId}");
            }

            taskItem.Title = request.Title;
            taskItem.Description = request.Description;
            taskItem.StateId = request.StateId;
            taskItem.AssigneeId = request.AssigneeId;
            taskItem.DueDate = request.DueDate;
            taskItem.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Tasks.Update(taskItem);
        }, cancellationToken);
    }
}
