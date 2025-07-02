using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetOverdueTaskItemsQueryHandler : IRequestHandler<GetOverdueTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskItemRepository _taskItemRepository;

    public GetOverdueTaskItemsQueryHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetOverdueTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var taskItems = await _taskItemRepository.GetOverdueTasksAsync(cancellationToken);
        return taskItems.ToDto();
    }
}
