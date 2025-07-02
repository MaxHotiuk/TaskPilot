using Application.Common.Dtos.Boards;
using MediatR;

namespace Application.Queries.Boards;

public record GetBoardByIdQuery(Guid Id) : IRequest<BoardDto?>;
