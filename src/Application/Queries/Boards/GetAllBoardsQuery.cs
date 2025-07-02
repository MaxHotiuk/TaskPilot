using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record GetAllBoardsQuery : IRequest<IEnumerable<BoardDto>>;
