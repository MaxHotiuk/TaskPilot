using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.BoardMembers;

public class UpdateBoardMemberRoleCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBoardMemberRoleCommand>
{
    public UpdateBoardMemberRoleCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateBoardMemberRoleCommand request, CancellationToken cancellationToken)
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
                Description = $"User '{username}' role was changed to '{request.Role}'."
            };
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            boardMember.Role = request.Role;
            boardMember.UpdatedAt = DateTime.UtcNow;

            unitOfWork.BoardMembers.Update(boardMember);

            await unitOfWork.Boards.TouchBoardAsync(request.BoardId, cancellationToken);
        }, cancellationToken);
    }
}
