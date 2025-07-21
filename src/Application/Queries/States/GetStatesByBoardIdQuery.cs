using Domain.Dtos.States;
using MediatR;

namespace Application.Queries.States;

public record GetStatesByBoardIdQuery(Guid BoardId) : IRequest<IEnumerable<StateDto>>;
