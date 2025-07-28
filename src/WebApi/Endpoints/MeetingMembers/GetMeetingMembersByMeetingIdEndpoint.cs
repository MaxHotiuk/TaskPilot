using Application.Queries.MeetingMembers;
using Domain.Dtos.Meetings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.MeetingMembers;

public class GetMeetingMembersByMeetingIdEndpoint : EndpointBaseWithRequest<GetMeetingMembersByMeetingIdQuery, IEnumerable<MeetingMemberDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/meetings/{meetingId:guid}/members", async (
                Guid meetingId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetMeetingMembersByMeetingIdQuery(meetingId);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetMeetingMembersByMeetingId")
            .WithTags("MeetingMembers")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(GetMeetingMembersByMeetingIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
