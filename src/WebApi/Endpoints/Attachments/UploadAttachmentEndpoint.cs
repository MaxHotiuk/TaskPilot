using Application.Abstractions.Storage;
using Domain.Dtos.Attachments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Attachments;

public class UploadAttachmentEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/attachments/{entityId}", async (Guid entityId, IFormFile file, IAttachmentService attachmentService, CancellationToken cancellationToken) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("No file uploaded.");
            using var stream = file.OpenReadStream();
            var attachment = await attachmentService.UploadAsync(entityId, stream, file.FileName, file.ContentType, cancellationToken);
            return Results.Ok(attachment);
        })
        .WithName("UploadAttachment")
        .WithTags("Attachments")
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<AttachmentDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
