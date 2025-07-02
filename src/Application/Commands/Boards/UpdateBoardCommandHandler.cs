using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Boards;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBoardCommandHandler(IBoardRepository boardRepository, IUnitOfWork unitOfWork)
    {
        _boardRepository = boardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (board is null)
        {
            throw new NotFoundException($"Board with ID {request.Id} was not found");
        }

        board.Name = request.Name;
        board.Description = request.Description;
        board.UpdatedAt = DateTime.UtcNow;

        _boardRepository.Update(board);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
