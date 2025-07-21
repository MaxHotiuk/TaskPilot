using Application.Queries.Notifications;
using Domain.Common.Authorization;
using MediatR;

namespace WebApi.Endpoints.Notifications;

public class GetUnreadNotificationsCountEndpoint : EndpointBaseWithRequest<GetUnreadNotificationsCountQuery, int>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications/unread-count", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetUnreadNotificationsCountQuery(userId), mediator, cancellationToken);
            })
            .WithName("GetUnreadNotificationsCount")
            .WithTags("Notifications")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetUnreadNotificationsCountQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
