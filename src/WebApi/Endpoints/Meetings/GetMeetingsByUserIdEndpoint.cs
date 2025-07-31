using Application.Queries.Meetings;
using Domain.Dtos.Meetings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Meetings;

public class GetMeetingsByUserIdEndpoint : EndpointBaseWithRequest<GetMeetingsByUserIdQuery, IEnumerable<MeetingDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/meetings", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetMeetingsByUserIdQuery(userId);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetMeetingsByUserId")
            .WithTags("Meetings")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(GetMeetingsByUserIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
