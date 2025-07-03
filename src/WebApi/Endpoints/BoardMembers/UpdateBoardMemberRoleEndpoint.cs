using Application.Commands.BoardMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.BoardMembers;

public class UpdateBoardMemberRoleEndpoint : EndpointBaseWithRequest<UpdateBoardMemberRoleCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/boards/{boardId:guid}/members/{userId:guid}/role", async (
                Guid boardId,
                Guid userId,
                UpdateBoardMemberRoleRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateBoardMemberRoleCommand(boardId, userId, dto.Role);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateBoardMemberRole")
            .WithTags("BoardMembers")
            .RequireAuthorization(Policies.RequireBoardOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateBoardMemberRoleCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record UpdateBoardMemberRoleRequestDto(string Role);
