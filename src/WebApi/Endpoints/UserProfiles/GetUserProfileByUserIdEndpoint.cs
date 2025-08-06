using Application.Queries.UserProfile;
using Domain.Dtos.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.UserProfiles;

public class GetUserProfileByUserIdEndpoint : EndpointBaseWithRequest<GetUserProfileByUserIdQuery, UserProfileDto?>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/profile", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUserProfileByUserIdQuery(userId);
                return await HandleAsync(request, mediator, cancellationToken);
            })
            .WithName("GetUserProfileByUserId")
            .WithTags("UserProfiles")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<UserProfileDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetUserProfileByUserIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
