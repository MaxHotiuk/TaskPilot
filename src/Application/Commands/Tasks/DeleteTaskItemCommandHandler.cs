using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Tasks;

public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskItemCommandHandler(ITaskItemRepository taskItemRepository, IUnitOfWork unitOfWork)
    {
        _taskItemRepository = taskItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (taskItem is null)
        {
            throw new NotFoundException($"Task with ID {request.Id} was not found");
        }

        _taskItemRepository.Remove(taskItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
