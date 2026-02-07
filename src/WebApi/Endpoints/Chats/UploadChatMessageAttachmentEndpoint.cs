using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Common.Mappings;
using Domain.Common.Authorization;
using Domain.Dtos.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Chats;

public class UploadChatMessageAttachmentEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chats/{chatId:guid}/messages/{messageId:guid}/attachments", async (
                Guid chatId,
                Guid messageId,
                Guid userId,
                IFormFile file,
                IUnitOfWorkFactory unitOfWorkFactory,
                IAttachmentService attachmentService,
                IChatNotifier chatNotifier,
                CancellationToken cancellationToken) =>
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }

                using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);
                var isMember = await unitOfWork.ChatMembers.IsMemberAsync(chatId, userId, cancellationToken);
                if (!isMember)
                {
                    return Results.Forbid();
                }

                var message = await unitOfWork.ChatMessages.GetByIdAsync(messageId, cancellationToken);
                if (message is null || message.ChatId != chatId)
                {
                    return Results.NotFound();
                }

                using var stream = file.OpenReadStream();
                var attachment = await attachmentService.UploadAsync(messageId, stream, file.FileName, file.ContentType, cancellationToken);

                message.HasAttachments = true;
                message.UpdatedAt = DateTime.UtcNow;
                unitOfWork.ChatMessages.Update(message);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var sender = await unitOfWork.Users.GetByIdAsync(message.SenderId, cancellationToken);
                if (sender is not null)
                {
                    message.Sender = sender;
                    await chatNotifier.NotifyChatMessageAsync(chatId, message.ToDto());
                }

                return Results.Ok(attachment);
            })
            .WithName("UploadChatMessageAttachment")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<AttachmentDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
