using Application.Queries.Organizations;
using Application.Abstractions.Authentication;
using Domain.Dtos.Organizations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Organizations;

public class GetOrganizationMembersEndpoint : EndpointBaseWithRequest<GetOrganizationMembersQuery, IEnumerable<OrganizationMemberDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/organizations/{organizationId:guid}/members", async (
                Guid organizationId,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var requesterId = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(requesterId))
                    return Results.Unauthorized();

                var query = new GetOrganizationMembersQuery(organizationId, Guid.Parse(requesterId));
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetOrganizationMembers")
            .WithTags("Organizations")
            .RequireAuthorization()
            .Produces<IEnumerable<OrganizationMemberDto>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetOrganizationMembersQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
