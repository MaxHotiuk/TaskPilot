using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Mappings;
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
        return board?.ToDto();
    }
}
