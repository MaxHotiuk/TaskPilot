using Application.Commands.MeetingMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.MeetingMembers;

public class DeleteMeetingMemberEndpoint : EndpointBaseWithRequest<RemoveMeetingMemberCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/meetings/{meetingId:guid}/members/{userId:guid}", async (
                Guid meetingId,
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveMeetingMemberCommand(meetingId, userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("DeleteMeetingMember")
            .WithTags("MeetingMembers")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(RemoveMeetingMemberCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
