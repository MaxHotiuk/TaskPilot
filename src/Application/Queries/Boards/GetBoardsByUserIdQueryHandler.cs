using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Mappings;
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
        return boards.ToDto();
    }
}
