using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.States;

public class GetStatesByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetStatesByBoardIdQuery, IEnumerable<StateDto>>
{
    public GetStatesByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<StateDto>> Handle(GetStatesByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var states = await unitOfWork.States.GetStatesByBoardIdAsync(request.BoardId, cancellationToken);
            return states.ToDto().OrderBy(s => s.Order);
        }, cancellationToken);
    }
}
