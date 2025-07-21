using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Notifications;

public class MarkAllNotificationsAsReadCommandHandler : BaseCommandHandler, IRequestHandler<MarkAllNotificationsAsReadCommand>
{
    public MarkAllNotificationsAsReadCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            await unitOfWork.Notifications.MarkAllAsReadAsync(request.UserId, cancellationToken);
        }, cancellationToken);
    }
}
