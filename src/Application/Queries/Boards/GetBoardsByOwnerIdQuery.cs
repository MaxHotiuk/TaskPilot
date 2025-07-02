using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record GetBoardsByOwnerIdQuery(Guid OwnerId) : IRequest<IEnumerable<BoardDto>>;
