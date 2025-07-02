using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.States;

public class GetAllStatesQueryHandler : IRequestHandler<GetAllStatesQuery, IEnumerable<StateDto>>
{
    private readonly IStateRepository _stateRepository;

    public GetAllStatesQueryHandler(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<IEnumerable<StateDto>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
    {
        var states = await _stateRepository.GetAllAsync(cancellationToken);
        return states.ToDto().OrderBy(s => s.Order);
    }
}
