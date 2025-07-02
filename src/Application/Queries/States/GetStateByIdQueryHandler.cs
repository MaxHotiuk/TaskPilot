using Application.Abstractions.Persistence;
using Application.Common.Dtos.States;
using Application.Common.Mappings;
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
        return state?.ToDto();
    }
}
