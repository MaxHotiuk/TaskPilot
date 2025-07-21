using Application.Abstractions.Persistence;
using Domain.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTaskItemByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetTaskItemByIdQuery, TaskItemDto?>
{
    public GetTaskItemByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<TaskItemDto?> Handle(GetTaskItemByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var taskItem = await unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
            return taskItem?.ToDto();
        }, cancellationToken);
    }
}
