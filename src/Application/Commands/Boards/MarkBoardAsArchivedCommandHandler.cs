using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Boards;

public class MarkBoardAsArchivedCommandHandler : BaseCommandHandler, IRequestHandler<MarkBoardAsArchivedCommand, bool>
{
    public MarkBoardAsArchivedCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(MarkBoardAsArchivedCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            return await unitOfWork.Boards.MarkBoardAsArchivedAsync(request.BoardId, request.ArchivalReason, cancellationToken);
        }, cancellationToken);
    }
}
