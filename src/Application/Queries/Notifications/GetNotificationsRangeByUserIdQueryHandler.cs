using Application.Abstractions.Persistence;
using Domain.Dtos.Notifications;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Notifications;

public class GetNotificationsRangeByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetNotificationsRangeByUserIdQuery, IEnumerable<NotificationDto>>
{
    public GetNotificationsRangeByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsRangeByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var notifications = await unitOfWork.Notifications.GetNotificationsRangeByUserIdAsync(
                request.UserId,
                request.Page,
                request.PageSize,
                cancellationToken);
            return notifications.Select(n => n.ToDto());
        }, cancellationToken);
    }
}
