using Application.Abstractions.Storage;
using Domain.Common.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Avatars;

public class DeleteAvatarEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/avatars/{userId}", async (Guid userId, IAvatarService avatarService, CancellationToken cancellationToken) =>
        {
            try
            {
                await avatarService.DeleteAsync(userId, cancellationToken);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("DeleteAvatar")
        .WithTags("Avatars")
        .DisableAntiforgery()
        .RequireAuthorization(Policies.RequireSelfUpdate)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
