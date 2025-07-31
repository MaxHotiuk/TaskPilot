using Application.Queries.Notifications;
using MediatR;
using Domain.Dtos.Notifications;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Notifications;

public class GetNotificationsByUserIdEndpoint : EndpointBaseWithRequest<GetNotificationsByUserIdQuery, IEnumerable<NotificationDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications/{userId:guid}", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetNotificationsByUserIdQuery(userId), mediator, cancellationToken);
            })
            .WithName("GetNotificationsByUserId")
            .WithTags("Notifications")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<NotificationDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetNotificationsByUserIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
