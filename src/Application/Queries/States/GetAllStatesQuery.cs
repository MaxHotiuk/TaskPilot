using Application.Common.Dtos.States;
using MediatR;

namespace Application.Queries.States;

public record GetAllStatesQuery : IRequest<IEnumerable<StateDto>>;
