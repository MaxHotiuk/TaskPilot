using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class CreateChatEndpoint : EndpointBaseWithRequest<CreateChatCommand, Guid>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chats", async (
                CreateChatRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateChatCommand(
                    dto.OrganizationId,
                    dto.CreatedById,
                    dto.Type,
                    dto.Name,
                    dto.MemberIds
                );

                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateChat")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateChatCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/chats/{result}", result);
    }
}
