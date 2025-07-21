using Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Attachments;

public class DeleteAttachmentEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/attachments/{fileName}", async (string fileName, IAttachmentService attachmentService, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Results.BadRequest("File name is required.");

            await attachmentService.DeleteAsync(fileName, cancellationToken);
            return Results.NoContent();
        })
        .WithName("DeleteAttachment")
        .WithTags("Attachments")
        .DisableAntiforgery()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
