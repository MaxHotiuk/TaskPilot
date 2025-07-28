using Application.Commands.MeetingMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.MeetingMembers;

public class AddMeetingMemberEndpoint : EndpointBaseWithRequest<AddMeetingMemberCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/meetings/{meetingId:guid}/members", async (
                Guid meetingId,
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new AddMeetingMemberCommand(meetingId, userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("AddMeetingMember")
            .WithTags("MeetingMembers")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(AddMeetingMemberCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
