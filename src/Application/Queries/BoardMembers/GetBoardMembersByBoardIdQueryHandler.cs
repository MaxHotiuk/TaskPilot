using Application.Abstractions.Persistence;
using Application.Common.Dtos.BoardMembers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.BoardMembers;

public class GetBoardMembersByBoardIdQueryHandler : IRequestHandler<GetBoardMembersByBoardIdQuery, IEnumerable<BoardMemberDto>>
{
    private readonly IBoardMemberRepository _boardMemberRepository;

    public GetBoardMembersByBoardIdQueryHandler(IBoardMemberRepository boardMemberRepository)
    {
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<IEnumerable<BoardMemberDto>> Handle(GetBoardMembersByBoardIdQuery request, CancellationToken cancellationToken)
    {
        var boardMembers = await _boardMemberRepository.GetMembersByBoardIdAsync(request.BoardId, cancellationToken);
        return boardMembers.ToDto();
    }
}
