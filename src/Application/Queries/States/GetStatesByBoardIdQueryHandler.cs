using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
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
