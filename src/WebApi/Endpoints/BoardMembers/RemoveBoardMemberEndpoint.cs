using Application.Commands.BoardMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.BoardMembers;

public class RemoveBoardMemberEndpoint : EndpointBaseWithRequest<RemoveBoardMemberCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/boards/{boardId:guid}/members/{userId:guid}", async (
                Guid boardId,
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveBoardMemberCommand(boardId, userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("RemoveBoardMember")
            .WithTags("BoardMembers")
            .RequireAuthorization(Policies.RequireBoardOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(RemoveBoardMemberCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
