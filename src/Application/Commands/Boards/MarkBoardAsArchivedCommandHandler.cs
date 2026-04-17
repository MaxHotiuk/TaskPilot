using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Boards;

public class MarkBoardAsArchivedCommandHandler : BaseCommandHandler, IRequestHandler<MarkBoardAsArchivedCommand, bool>
{
    private readonly IAiSyncEnqueuer _aiSyncEnqueuer;

    public MarkBoardAsArchivedCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IAiSyncEnqueuer aiSyncEnqueuer)
        : base(unitOfWorkFactory)
    {
        _aiSyncEnqueuer = aiSyncEnqueuer;
    }

    public async Task<bool> Handle(MarkBoardAsArchivedCommand request, CancellationToken cancellationToken)
    {
        var result = await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.Boards.MarkBoardAsArchivedAsync(request.BoardId, request.ArchivalReason, cancellationToken);
        }, cancellationToken);

        _aiSyncEnqueuer.EnqueueSyncBoard(request.BoardId);
        return result;
    }
}
