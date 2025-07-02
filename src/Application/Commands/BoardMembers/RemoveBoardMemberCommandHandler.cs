using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.BoardMembers;

public class RemoveBoardMemberCommandHandler : IRequestHandler<RemoveBoardMemberCommand>
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveBoardMemberCommandHandler(IBoardMemberRepository boardMemberRepository, IUnitOfWork unitOfWork)
    {
        _boardMemberRepository = boardMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveBoardMemberCommand request, CancellationToken cancellationToken)
    {
        var boardMember = await _boardMemberRepository.GetBoardMemberAsync(request.BoardId, request.UserId, cancellationToken);
        
        if (boardMember is null)
        {
            throw new NotFoundException($"Board member not found for board {request.BoardId} and user {request.UserId}");
        }

        _boardMemberRepository.Remove(boardMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
