using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tags;

public class CreateTagCommandHandler : BaseCommandHandler, IRequestHandler<CreateTagCommand, int>
{
    public CreateTagCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            var existingTag = await unitOfWork.Tags.GetTagByBoardAndNameAsync(request.BoardId, request.Name, cancellationToken);
            if (existingTag is not null)
            {
                throw new ValidationException($"Tag with name '{request.Name}' already exists in this board");
            }

            var tag = new Tag
            {
                BoardId = request.BoardId,
                Name = request.Name,
                Color = request.Color,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Tags.AddAsync(tag, cancellationToken);

            board.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Boards.Update(board);

            return tag.Id;
        }, cancellationToken);
    }
}
