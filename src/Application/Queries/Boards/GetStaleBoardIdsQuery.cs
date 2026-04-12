using MediatR;

namespace Application.Queries.Boards;

public record GetStaleBoardIdsQuery(int StaleDays = 30) : IRequest<IEnumerable<Guid>>;
