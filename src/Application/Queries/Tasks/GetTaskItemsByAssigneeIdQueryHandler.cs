using Application.Abstractions.Persistence;
using Domain.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemsByAssigneeIdQueryHandler : BaseQueryHandler, IRequestHandler<GetTaskItemsByAssigneeIdQuery, IEnumerable<TaskItemDto>>
{
    public GetTaskItemsByAssigneeIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTaskItemsByAssigneeIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var taskItems = await unitOfWork.Tasks.GetTasksByAssigneeIdAsync(request.AssigneeId, cancellationToken);
            return taskItems.ToDto();
        }, cancellationToken);
    }
}
