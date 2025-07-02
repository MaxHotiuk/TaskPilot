using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
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

        return states.Select(state => new StateDto
        {
            Id = state.Id,
            BoardId = state.BoardId,
            Name = state.Name,
            Order = state.Order,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        }).OrderBy(s => s.Order);
    }
}
