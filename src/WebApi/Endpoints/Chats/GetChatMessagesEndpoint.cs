using Application.Queries.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class GetChatMessagesEndpoint : EndpointBaseWithRequest<GetChatMessagesQuery, IEnumerable<ChatMessageDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/chats/{chatId:guid}/messages", async (
                Guid chatId,
                Guid userId,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetChatMessagesQuery(chatId, userId, page, pageSize);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetChatMessages")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<ChatMessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetChatMessagesQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
