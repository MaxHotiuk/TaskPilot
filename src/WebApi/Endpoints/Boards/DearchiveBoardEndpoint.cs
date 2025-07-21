using Application.Abstractions.Archivation;
using Domain.Common.Authorization;
using MediatR;

namespace WebApi.Endpoints.Boards;

public class DearchiveBoardEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/dearchive", async (
            Guid boardId,
            IArchivalService archivalService,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            string? dearchivalReason = null;
            if (httpContext.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(httpContext.Request.Body);
                dearchivalReason = await reader.ReadToEndAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(dearchivalReason)) dearchivalReason = null;
            }

            var job = await archivalService.MarkBoardAsDearchivedAsync(boardId, dearchivalReason, cancellationToken);
            return Results.Ok(job);
        })
        .WithName("DearchiveBoard")
        .WithTags("Boards")
        .RequireAuthorization(Policies.RequireBoardOwner)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
