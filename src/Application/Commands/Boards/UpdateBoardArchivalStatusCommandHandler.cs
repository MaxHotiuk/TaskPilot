using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Boards;

public class UpdateBoardArchivalStatusCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBoardArchivalStatusCommand, bool>
{
    public UpdateBoardArchivalStatusCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(UpdateBoardArchivalStatusCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.Boards.UpdateBoardArchivalStatusAsync(request.BoardId, request.IsArchived, request.ArchivedAt, request.ArchivalReason, cancellationToken);
        }, cancellationToken);
    }
}
