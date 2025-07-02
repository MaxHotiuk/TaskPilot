using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record GetBoardsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<BoardDto>>;
