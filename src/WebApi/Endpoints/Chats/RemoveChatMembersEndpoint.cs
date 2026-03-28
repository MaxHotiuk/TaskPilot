using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class RemoveChatMembersEndpoint : EndpointBaseWithRequest<RemoveChatMembersCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chats/{chatId:guid}/members/remove", async (
                Guid chatId,
                UpdateChatMembersRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveChatMembersCommand(chatId, dto.UserId, dto.MemberIds);
                await HandleAsync(command, mediator, cancellationToken);
                return Results.NoContent();
            })
            .WithName("RemoveChatMembers")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(RemoveChatMembersCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
