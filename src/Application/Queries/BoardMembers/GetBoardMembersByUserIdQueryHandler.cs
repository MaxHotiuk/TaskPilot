using Application.Abstractions.Persistence;
using Application.Common.Dtos.BoardMembers;
using MediatR;

namespace Application.Queries.BoardMembers;

public class GetBoardMembersByUserIdQueryHandler : IRequestHandler<GetBoardMembersByUserIdQuery, IEnumerable<BoardMemberDto>>
{
    private readonly IBoardMemberRepository _boardMemberRepository;

    public GetBoardMembersByUserIdQueryHandler(IBoardMemberRepository boardMemberRepository)
    {
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<IEnumerable<BoardMemberDto>> Handle(GetBoardMembersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var boardMembers = await _boardMemberRepository.GetBoardsByUserIdAsync(request.UserId, cancellationToken);

        return boardMembers.Select(boardMember => new BoardMemberDto
        {
            BoardId = boardMember.BoardId,
            UserId = boardMember.UserId,
            Role = boardMember.Role,
            CreatedAt = boardMember.CreatedAt,
            UpdatedAt = boardMember.UpdatedAt
        });
    }
}
