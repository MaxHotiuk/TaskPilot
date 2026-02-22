using Application.Commands.BoardMembers;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.BoardMembers;

public class AddBoardMemberEndpoint : EndpointBaseWithRequest<AddBoardMemberCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/members", async (
                Guid boardId,
                AddBoardMemberRequestDto dto,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var inviterId = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(inviterId))
                    return Results.Unauthorized();

                var command = new AddBoardMemberCommand(
                    boardId,
                    dto.UserId,
                    dto.Role,
                    Guid.Parse(inviterId)
                );

                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("AddBoardMember")
            .WithTags("BoardMembers")
            .RequireAuthorization(Policies.RequireBoardOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(AddBoardMemberCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record AddBoardMemberRequestDto(Guid UserId, string Role = "Member");
