using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Boards;

public class CreateBoardCommandHandler : BaseCommandHandler, IRequestHandler<CreateBoardCommand, Guid>
{

    private readonly IBoardNotifier _boardNotifier;

    public CreateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = new Board
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Boards.AddAsync(board, cancellationToken);

            var organizationIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(request.OwnerId, cancellationToken);
            var organizationId = organizationIds.FirstOrDefault();
            if (organizationId == Guid.Empty)
            {
                throw new ValidationException("Board owner must belong to an organization to create a board chat.");
            }

            var boardChat = new Chat
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                BoardId = board.Id,
                Name = board.Name,
                Type = ChatType.Board,
                CreatedById = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Chats.AddAsync(boardChat, cancellationToken);

            var ownerChatMember = new ChatMember
            {
                ChatId = boardChat.Id,
                UserId = request.OwnerId,
                Role = ChatMemberRole.Owner,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.ChatMembers.AddAsync(ownerChatMember, cancellationToken);

            var backlogEntry = new Domain.Entities.Backlog
            {
                BoardId = board.Id,
                Description = $"Board '{board.Name}' was created."
            };
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            await _boardNotifier.NotifyBoardUpdatedAsync(board.Id.ToString(), new { action = "created", boardId = board.Id });
            return board.Id;
        }, cancellationToken);
    }
}
