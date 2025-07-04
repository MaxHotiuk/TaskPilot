using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
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

            boardMember.Role = request.Role;
            boardMember.UpdatedAt = DateTime.UtcNow;

            unitOfWork.BoardMembers.Update(boardMember);
        }, cancellationToken);
    }
}
