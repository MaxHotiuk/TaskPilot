using Application.Abstractions.Archivation;
using Domain.Common.Authorization;
using MediatR;

namespace WebApi.Endpoints.Boards;

public class ArchiveBoardEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/archive", async (
            Guid boardId,
            IArchivalService archivalService,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            string? archivalReason = null;
            if (httpContext.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(httpContext.Request.Body);
                archivalReason = await reader.ReadToEndAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(archivalReason)) archivalReason = null;
            }

            var job = await archivalService.MarkBoardAsArchivedAsync(boardId, archivalReason, cancellationToken);
            return Results.Ok(job);
        })
        .WithName("ArchiveBoard")
        .WithTags("Boards")
        .RequireAuthorization(Policies.RequireBoardOwner)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
