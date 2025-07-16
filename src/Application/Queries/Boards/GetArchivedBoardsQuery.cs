using Application.Common.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.Boards;

public record GetArchivedBoardsQuery() : IRequest<IEnumerable<BoardDto>>;
