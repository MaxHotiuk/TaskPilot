using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.States;

public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand>
{
    private readonly IStateRepository _stateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStateCommandHandler(IStateRepository stateRepository, IUnitOfWork unitOfWork)
    {
        _stateRepository = stateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        var state = await _stateRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (state is null)
        {
            throw new NotFoundException($"State with ID {request.Id} was not found");
        }

        _stateRepository.Remove(state);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
