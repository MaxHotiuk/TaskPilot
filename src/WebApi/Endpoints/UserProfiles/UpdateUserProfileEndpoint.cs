using Application.Commands.UserProfile;
using Domain.Dtos.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.UserProfiles;

public class UpdateUserProfileEndpoint : EndpointBaseWithRequest<UpdateUserProfileCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/userprofiles/{id:guid}", async (
                Guid id,
                UpdateUserProfileRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateUserProfileCommand(
                    id,
                    dto.Bio,
                    dto.JobTitle,
                    dto.Department,
                    dto.Location,
                    dto.PhoneNumber,
                    dto.AddToBoardAutomatically,
                    dto.ShowEmail,
                    dto.ShowPhoneNumber);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateUserProfile")
            .WithTags("UserProfiles")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateUserProfileCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
