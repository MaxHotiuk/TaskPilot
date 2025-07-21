using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.BoardMembers;

public class AddBoardMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddBoardMemberCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public AddBoardMemberCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new ValidationException($"User with ID {request.UserId} does not exist");
            }

            if (await unitOfWork.BoardMembers.IsMemberOfBoardAsync(request.BoardId, request.UserId, cancellationToken))
            {
                throw new ValidationException($"User {request.UserId} is already a member of board {request.BoardId}");
            }

            var boardMember = new BoardMember
            {
                BoardId = request.BoardId,
                UserId = request.UserId,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.BoardMembers.AddAsync(boardMember, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(boardMember.BoardId.ToString(), new { action = "addedUser", boardId = boardMember.BoardId });
        }, cancellationToken);
    }
}
