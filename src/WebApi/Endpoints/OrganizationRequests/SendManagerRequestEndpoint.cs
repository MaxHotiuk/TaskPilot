using Application.Commands.OrganizationRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.OrganizationRequests;

public class SendManagerRequestEndpoint : EndpointBaseWithRequest<SendManagerRequestCommand, Unit>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/organizations/{organizationId:guid}/manager-request", async (
                Guid organizationId,
                SendManagerRequestRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SendManagerRequestCommand(
                    request.UserId,
                    organizationId.ToString(),
                    request.Message);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("SendManagerRequest")
            .WithTags("Organizations")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SendManagerRequestCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record SendManagerRequestRequest(string UserId, string Message);
