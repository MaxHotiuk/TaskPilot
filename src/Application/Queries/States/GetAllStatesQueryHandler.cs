using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.States;

public class GetAllStatesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllStatesQuery, IEnumerable<StateDto>>
{
    public GetAllStatesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<StateDto>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var states = await unitOfWork.States.GetAllAsync(cancellationToken);
            return states.ToDto().OrderBy(s => s.Order);
        }, cancellationToken);
    }
}
