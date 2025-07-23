using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tags;

public class UpdateTagCommandHandler : BaseCommandHandler, IRequestHandler<UpdateTagCommand>
{
    public UpdateTagCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
            if (tag is null || tag.BoardId != request.BoardId)
            {
                throw new ValidationException($"Tag with ID {request.Id} does not exist in board {request.BoardId}");
            }

            var duplicateTag = await unitOfWork.Tags.GetTagByBoardAndNameAsync(request.BoardId, request.Name, cancellationToken);
            if (duplicateTag is not null && duplicateTag.Id != request.Id)
            {
                throw new ValidationException($"Tag with name '{request.Name}' already exists in this board");
            }

            tag.Name = request.Name;
            tag.Color = request.Color;
            tag.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Tags.Update(tag);
        }, cancellationToken);
    }
}
