using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Boards;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBoardCommandHandler(IBoardRepository boardRepository, IUnitOfWork unitOfWork)
    {
        _boardRepository = boardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (board is null)
        {
            throw new NotFoundException($"Board with ID {request.Id} was not found");
        }

        _boardRepository.Remove(board);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
