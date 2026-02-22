using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.Common.Authorization;
using Domain.Dtos.Avatars;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Chats;

public class UpdateChatAvatarEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/chats/{chatId:guid}/avatar", async (
                Guid chatId,
                Guid userId,
                IFormFile file,
                IUnitOfWorkFactory unitOfWorkFactory,
                IChatAvatarService chatAvatarService,
                CancellationToken cancellationToken) =>
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }

                using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);
                var chat = await unitOfWork.Chats.GetByIdAsync(chatId, cancellationToken);
                if (chat is null)
                {
                    return Results.NotFound();
                }

                if (chat.Type == ChatType.Private)
                {
                    return Results.BadRequest("Chat avatars are only supported for group or board chats.");
                }

                var isMember = await unitOfWork.ChatMembers.IsMemberAsync(chatId, userId, cancellationToken);
                if (!isMember)
                {
                    return Results.Forbid();
                }

                using var stream = file.OpenReadStream();
                try
                {
                    await chatAvatarService.DeleteAsync(chatId, cancellationToken);
                    var avatar = await chatAvatarService.UploadAsync(chatId, stream, file.ContentType, cancellationToken);
                    return Results.Ok(avatar);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("UpdateChatAvatar")
            .WithTags("Chats")
            .DisableAntiforgery()
            .RequireAuthorization(Policies.RequireUserRole)
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ChatAvatarDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
