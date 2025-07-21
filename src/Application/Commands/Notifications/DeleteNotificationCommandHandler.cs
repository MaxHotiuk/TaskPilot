using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Notifications;

public class DeleteNotificationCommandHandler : BaseCommandHandler, IRequestHandler<DeleteNotificationCommand>
{
    public DeleteNotificationCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var notification = await unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null)
            {
                throw new NotFoundException($"Notification with ID {request.NotificationId} not found.");
            }

            unitOfWork.Notifications.Remove(notification);
        }, cancellationToken);
    }
}
