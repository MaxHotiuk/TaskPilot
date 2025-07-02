using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.States;

public class GetStatesByBoardIdQueryHandler : IRequestHandler<GetStatesByBoardIdQuery, IEnumerable<StateDto>>
{
    private readonly IStateRepository _stateRepository;

    public GetStatesByBoardIdQueryHandler(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<IEnumerable<StateDto>> Handle(GetStatesByBoardIdQuery request, CancellationToken cancellationToken)
    {
        var states = await _stateRepository.GetStatesByBoardIdAsync(request.BoardId, cancellationToken);
        return states.ToDto().OrderBy(s => s.Order);
    }
}
