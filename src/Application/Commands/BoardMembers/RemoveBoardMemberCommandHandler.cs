using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.BoardMembers;

public class RemoveBoardMemberCommandHandler : BaseCommandHandler, IRequestHandler<RemoveBoardMemberCommand>
{
    public RemoveBoardMemberCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(RemoveBoardMemberCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var boardMember = await unitOfWork.BoardMembers.GetBoardMemberAsync(request.BoardId, request.UserId, cancellationToken);
            
            if (boardMember is null)
            {
                throw new NotFoundException($"Board member not found for board {request.BoardId} and user {request.UserId}");
            }

            unitOfWork.BoardMembers.Remove(boardMember);
        }, cancellationToken);
    }
}
