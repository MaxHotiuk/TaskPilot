using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace WebApi.Endpoints.Attachments;

public class GetAttachmentsEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/attachments/{entityId}", async (Guid entityId, IAttachmentService attachmentService, CancellationToken cancellationToken) =>
        {
            var attachments = await attachmentService.GetForEntityAsync(entityId, cancellationToken);
            if (attachments == null || !attachments.Any())
                return Results.NotFound();
            return Results.Ok(attachments);
        })
        .WithName("GetAttachments")
        .WithTags("Attachments")
        .DisableAntiforgery()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
