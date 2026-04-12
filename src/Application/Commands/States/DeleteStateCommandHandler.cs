using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.States;

public class DeleteStateCommandHandler : BaseCommandHandler, IRequestHandler<DeleteStateCommand>
{
    public DeleteStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var state = await unitOfWork.States.GetByIdAsync(request.Id, cancellationToken);
            
            if (state is null)
            {
                throw new NotFoundException($"State with ID {request.Id} was not found");
            }

            await unitOfWork.Boards.TouchBoardAsync(state.BoardId, cancellationToken);

            unitOfWork.States.Remove(state);
        }, cancellationToken);
    }
}
