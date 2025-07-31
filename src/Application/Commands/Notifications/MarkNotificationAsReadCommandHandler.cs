using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Notifications;

public class MarkNotificationAsReadCommandHandler : BaseCommandHandler, IRequestHandler<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            await unitOfWork.Notifications.MarkAsReadAsync(request.NotificationId, cancellationToken);
        }, cancellationToken);
    }
}
