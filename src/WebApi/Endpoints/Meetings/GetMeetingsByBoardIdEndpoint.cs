using Application.Queries.Meetings;
using Domain.Dtos.Meetings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Meetings;

public class GetMeetingsByBoardIdEndpoint : EndpointBaseWithRequest<GetMeetingsByBoardIdQuery, IEnumerable<MeetingDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId:guid}/meetings", async (
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetMeetingsByBoardIdQuery(boardId);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetMeetingsByBoardId")
            .WithTags("Meetings")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    public override async Task<IResult> HandleAsync(GetMeetingsByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
