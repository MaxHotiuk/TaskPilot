using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.Common.Authorization;
using Domain.Dtos.Avatars;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Chats;

public class GetChatAvatarEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/chats/{chatId:guid}/avatar", async (
                Guid chatId,
                Guid userId,
                IUnitOfWorkFactory unitOfWorkFactory,
                IChatAvatarService chatAvatarService,
                CancellationToken cancellationToken) =>
            {
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

                var avatar = await chatAvatarService.GetAsync(chatId, cancellationToken);
                return avatar is null ? Results.NotFound() : Results.Ok(avatar);
            })
            .WithName("GetChatAvatar")
            .WithTags("Chats")
            .DisableAntiforgery()
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<ChatAvatarDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
