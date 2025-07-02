using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardByIdQueryHandler : IRequestHandler<GetBoardByIdQuery, BoardDto?>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardByIdQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardDto?> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.Id, cancellationToken);

        if (board is null)
        {
            return null;
        }

        return new BoardDto
        {
            Id = board.Id,
            Name = board.Name,
            Description = board.Description,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
            UpdatedAt = board.UpdatedAt
        };
    }
}
