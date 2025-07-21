using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Avatars;

public class GetAvatarEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/avatars/{userId}", async (Guid userId, IAvatarService avatarService, CancellationToken cancellationToken) =>
        {
            var avatar = await avatarService.GetAsync(userId, cancellationToken);
            if (avatar == null)
                return Results.NotFound();
            return Results.Ok(avatar);
        })
        .WithName("GetAvatar")
        .WithTags("Avatars")
        .DisableAntiforgery()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
