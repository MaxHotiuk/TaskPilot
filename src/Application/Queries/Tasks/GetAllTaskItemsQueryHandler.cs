using Application.Abstractions.Persistence;
using Application.Common.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetAllTaskItemsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    public GetAllTaskItemsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTaskItemsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var taskItems = await unitOfWork.Tasks.GetAllAsync(cancellationToken);
            return taskItems.ToDto();
        }, cancellationToken);
    }
}
