using Application.Queries.Notifications;
using MediatR;
using Domain.Dtos.Notifications;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Notifications;

public class GetNotificationsRangeByUserIdEndpoint : EndpointBaseWithRequest<GetNotificationsRangeByUserIdQuery, IEnumerable<NotificationDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications/{userId:guid}/range", async (
                Guid userId,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetNotificationsRangeByUserIdQuery(userId, page, pageSize), mediator, cancellationToken);
            })
            .WithName("GetNotificationsRangeByUserId")
            .WithTags("Notifications")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<NotificationDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetNotificationsRangeByUserIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
