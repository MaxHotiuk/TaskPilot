using Application.Commands.MeetingMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.MeetingMembers;

public class UpdateMeetingMemberStatusEndpoint : EndpointBaseWithRequest<UpdateMeetingMemberStatusCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/meetings/{meetingId:guid}/members/{userId:guid}/status", async (
                Guid meetingId,
                Guid userId,
                string status,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateMeetingMemberStatusCommand(meetingId, userId, status);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateMeetingMemberStatus")
            .WithTags("MeetingMembers")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(UpdateMeetingMemberStatusCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
