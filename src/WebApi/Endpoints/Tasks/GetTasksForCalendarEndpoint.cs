using Application.Queries.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.Tasks;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Tasks;

public class GetTasksForCalendarEndpoint : EndpointBaseWithRequest<GetTasksForCalendarMonthQuery, IEnumerable<TaskCalendarItemDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/calendar", async (
                Guid userId,
                DateTime dayInMonth,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetTasksForCalendarMonthQuery(userId, dayInMonth), mediator, cancellationToken);
            })
            .WithName("GetTasksForCalendarMonth")
            .WithTags("Tasks")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetTasksForCalendarMonthQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
