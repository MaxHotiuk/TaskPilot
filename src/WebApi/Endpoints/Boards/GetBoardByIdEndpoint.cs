using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Boards;

namespace WebApi.Endpoints.Boards;

public class GetBoardByIdEndpoint : EndpointBaseWithRequest<GetBoardByIdQuery, BoardDto?>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetBoardByIdQuery(id), mediator, cancellationToken);
            })
            .WithName("GetBoardById")
            .WithTags("Boards")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetBoardByIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
