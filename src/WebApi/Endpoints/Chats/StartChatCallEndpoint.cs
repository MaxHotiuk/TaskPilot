using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class StartChatCallEndpoint : EndpointBaseWithRequest<StartChatCallCommand, StartChatCallResponseDto>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chats/{chatId:guid}/calls", async (
                Guid chatId,
                StartChatCallRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new StartChatCallCommand(chatId, dto.SenderId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("StartChatCall")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<StartChatCallResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(StartChatCallCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
