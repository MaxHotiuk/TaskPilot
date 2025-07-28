using Application.Commands.Meetings;
using Domain.Dtos.Meetings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Meetings;

public class CreateMeetingEndpoint : EndpointBaseWithRequest<CreateMeetingCommand, string>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/meetings", async (
                CreateMeetingRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateMeetingCommand(dto);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateMeeting")
            .WithTags("Meetings")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<string>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateMeetingCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/meetings/{result}", result);
    }
}
