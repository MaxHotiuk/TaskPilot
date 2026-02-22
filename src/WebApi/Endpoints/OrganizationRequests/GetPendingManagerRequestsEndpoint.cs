using Application.Queries.OrganizationRequests;
using Domain.Common.Authorization;
using Domain.Dtos.OrganizationRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.OrganizationRequests;

public class GetPendingManagerRequestsEndpoint : EndpointBaseWithRequest<GetPendingManagerRequestsQuery, IEnumerable<ManagerRequestDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/organizations/manager-requests/pending", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPendingManagerRequestsQuery();
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetPendingManagerRequests")
            .WithTags("Organizations")
            .RequireAuthorization(Policies.RequireAdminRole)
            .Produces<IEnumerable<ManagerRequestDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetPendingManagerRequestsQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
