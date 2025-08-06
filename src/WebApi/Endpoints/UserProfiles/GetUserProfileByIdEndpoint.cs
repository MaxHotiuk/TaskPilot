using Application.Queries.UserProfile;
using Domain.Dtos.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.UserProfiles;

public class GetUserProfileByIdEndpoint : EndpointBaseWithRequest<GetUserProfileByIdQuery, UserProfileDto?>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/userprofiles/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUserProfileByIdQuery(id);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetUserProfileById")
            .WithTags("UserProfiles")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<UserProfileDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetUserProfileByIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
