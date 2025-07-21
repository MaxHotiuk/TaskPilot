using Application.Abstractions.Persistence;
using Domain.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemsByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetTaskItemsByBoardIdQuery, IEnumerable<TaskItemDto>>
{
    public GetTaskItemsByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTaskItemsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var taskItems = await unitOfWork.Tasks.GetTasksByBoardIdAsync(request.BoardId, cancellationToken);
            return taskItems.ToDto();
        }, cancellationToken);
    }
}
