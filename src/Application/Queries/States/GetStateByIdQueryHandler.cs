using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.States;

public class GetStateByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetStateByIdQuery, StateDto?>
{
    public GetStateByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<StateDto?> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var state = await unitOfWork.States.GetByIdAsync(request.Id, cancellationToken);
            return state?.ToDto();
        }, cancellationToken);
    }
}
