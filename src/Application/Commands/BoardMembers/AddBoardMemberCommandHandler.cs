using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.BoardMembers;

public class AddBoardMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddBoardMemberCommand>
{
    public AddBoardMemberCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            // Validate board exists
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            // Validate user exists
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new ValidationException($"User with ID {request.UserId} does not exist");
            }

            // Check if user is already a member
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
        }, cancellationToken);
    }
}
