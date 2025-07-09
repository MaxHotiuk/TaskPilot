using Application.Abstractions.Storage;
using Application.Common.Dtos.Avatars;
using Domain.Common.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Avatars;

public class UploadAvatarEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/avatars/{userId}", async (Guid userId, IFormFile file, IAvatarService avatarService, CancellationToken cancellationToken) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("No file uploaded.");
            using var stream = file.OpenReadStream();
            try
            {
                var avatar = await avatarService.UploadAsync(userId, stream, file.ContentType, cancellationToken);
                return Results.Ok(avatar);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("UploadAvatar")
        .WithTags("Avatars")
        .DisableAntiforgery()
        .RequireAuthorization(Policies.RequireSelfUpdate)
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AvatarDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
