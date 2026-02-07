using Application.Queries.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class GetChatsForUserEndpoint : EndpointBaseWithRequest<GetChatsForUserQuery, IEnumerable<ChatDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/chats", async (
                Guid userId,
                Guid? organizationId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetChatsForUserQuery(userId, organizationId);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetChatsForUser")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<ChatDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetChatsForUserQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
