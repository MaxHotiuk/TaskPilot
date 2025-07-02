using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemsByAssigneeIdQueryHandler : IRequestHandler<GetTaskItemsByAssigneeIdQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public GetTaskItemsByAssigneeIdQueryHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTaskItemsByAssigneeIdQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetTasksByAssigneeIdAsync(request.AssigneeId, cancellationToken);
        return taskItems.ToDto();
    }
}
