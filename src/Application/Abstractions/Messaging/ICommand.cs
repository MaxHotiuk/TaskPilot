using MediatR;

namespace Application.Abstractions.Messaging;

// Command without response
public interface ICommand : IRequest
{
}

// Command with response
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}