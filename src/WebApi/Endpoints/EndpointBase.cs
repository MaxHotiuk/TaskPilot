using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints;

public abstract class EndpointBase
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    protected IResult HandleResult<T>(T? result)
    {
        if (result is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
    }
}

public abstract class EndpointBaseWithRequest<TRequest> : EndpointBase
    where TRequest : IRequest
{
    public abstract Task<IResult> HandleAsync(TRequest request, IMediator mediator, CancellationToken cancellationToken);
}

public abstract class EndpointBaseWithRequest<TRequest, TResponse> : EndpointBase
    where TRequest : IRequest<TResponse>
{
    public abstract Task<IResult> HandleAsync(TRequest request, IMediator mediator, CancellationToken cancellationToken);
}
