using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardsByUserIdQueryHandler : IRequestHandler<GetBoardsByUserIdQuery, IEnumerable<BoardDto>>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardsByUserIdQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetBoardsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var boards = await _boardRepository.GetBoardsByUserIdAsync(request.UserId, cancellationToken);

        return boards.Select(board => new BoardDto
        {
            Id = board.Id,
            Name = board.Name,
            Description = board.Description,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
            UpdatedAt = board.UpdatedAt
        });
    }
}
