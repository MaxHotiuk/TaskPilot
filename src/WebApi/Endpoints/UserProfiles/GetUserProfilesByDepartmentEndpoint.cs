using Application.Queries.UserProfile;
using Domain.Dtos.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.UserProfiles;

public class GetUserProfilesByDepartmentEndpoint : EndpointBaseWithRequest<GetUserProfilesByDepartmentQuery, IEnumerable<UserProfileDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/userprofiles/department/{department}", async (
                string department,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUserProfilesByDepartmentQuery(department);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetUserProfilesByDepartment")
            .WithTags("UserProfiles")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<UserProfileDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetUserProfilesByDepartmentQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
