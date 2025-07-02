using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardsByOwnerIdQueryHandler : IRequestHandler<GetBoardsByOwnerIdQuery, IEnumerable<BoardDto>>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardsByOwnerIdQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetBoardsByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        var boards = await _boardRepository.GetBoardsByOwnerIdAsync(request.OwnerId, cancellationToken);
        return boards.ToDto();
    }
}
