using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;

namespace Application.Queries.Notifications;

public class GetUnreadNotificationsCountQueryHandler : BaseQueryHandler, IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    public GetUnreadNotificationsCountQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Notifications.GetUnreadCountByUserIdAsync(request.UserId, cancellationToken);
        }, cancellationToken);
    }
}
