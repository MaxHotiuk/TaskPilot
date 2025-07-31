using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tags;

public class DeleteTagCommandHandler : BaseCommandHandler, IRequestHandler<DeleteTagCommand>
{
    public DeleteTagCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
            if (tag is null || tag.BoardId != request.BoardId)
            {
                throw new ValidationException($"Tag with ID {request.Id} does not exist in board {request.BoardId}");
            }

            unitOfWork.Tags.Remove(tag);
        }, cancellationToken);
    }
}
