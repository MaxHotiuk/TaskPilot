using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Notifications;

public class CreateNotificationCommandHandler : BaseCommandHandler, IRequestHandler<CreateNotificationCommand>
{
    public CreateNotificationCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var notification = unitOfWork.Notifications.BuildNotification(
                request.UserId,
                request.Type,
                request.BoardId,
                request.TaskId,
                request.CustomText,
                request.BoardName,
                request.TaskName,
                request.UserComment
            );
            await unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        }, cancellationToken);
    }
}
