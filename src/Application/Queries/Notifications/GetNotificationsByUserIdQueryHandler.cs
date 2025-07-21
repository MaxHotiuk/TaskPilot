using Application.Abstractions.Persistence;
using Domain.Dtos.Notifications;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Notifications;

public class GetNotificationsByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetNotificationsByUserIdQuery, IEnumerable<NotificationDto>>
{
    public GetNotificationsByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var notifications = await unitOfWork.Notifications.GetNotificationsByUserIdAsync(request.UserId, cancellationToken);
            return notifications.Select(n => n.ToDto());
        }, cancellationToken);
    }
}
