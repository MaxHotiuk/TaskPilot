using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
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

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            var username = user?.Username ?? request.UserId.ToString();

            var backlogEntry = new Backlog
            {
                BoardId = request.BoardId,
                Description = $"User '{username}' was removed from the board."
            };
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            unitOfWork.BoardMembers.Remove(boardMember);
        }, cancellationToken);
    }
}
