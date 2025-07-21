using Domain.Dtos.States;
using MediatR;

namespace Application.Queries.States;

public record GetStateByIdQuery(int Id) : IRequest<StateDto?>;
