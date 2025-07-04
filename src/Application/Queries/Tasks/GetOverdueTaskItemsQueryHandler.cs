using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetOverdueTaskItemsQueryHandler : BaseQueryHandler, IRequestHandler<GetOverdueTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    public GetOverdueTaskItemsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetOverdueTaskItemsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var taskItems = await unitOfWork.Tasks.GetOverdueTasksAsync(cancellationToken);
            return taskItems.ToDto();
        }, cancellationToken);
    }
}
