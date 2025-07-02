using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using MediatR;

namespace Application.Queries.States;

public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, StateDto?>
{
    private readonly IStateRepository _stateRepository;

    public GetStateByIdQueryHandler(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<StateDto?> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        var state = await _stateRepository.GetByIdAsync(request.Id, cancellationToken);

        if (state is null)
        {
            return null;
        }

        return new StateDto
        {
            Id = state.Id,
            BoardId = state.BoardId,
            Name = state.Name,
            Order = state.Order,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }
}
