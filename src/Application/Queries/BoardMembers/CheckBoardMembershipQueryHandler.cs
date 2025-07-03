using Application.Abstractions.Persistence;
using MediatR;

namespace Application.Queries.BoardMembers;

public class CheckBoardMembershipQueryHandler : IRequestHandler<CheckBoardMembershipQuery, bool>
{
    private readonly IBoardMemberRepository _boardMemberRepository;

    public CheckBoardMembershipQueryHandler(IBoardMemberRepository boardMemberRepository)
    {
        _boardMemberRepository = boardMemberRepository;
    }

    public async Task<bool> Handle(CheckBoardMembershipQuery request, CancellationToken cancellationToken)
    {
        return await _boardMemberRepository.IsMemberOfBoardAsync(request.BoardId, request.UserId, cancellationToken);
    }
}
