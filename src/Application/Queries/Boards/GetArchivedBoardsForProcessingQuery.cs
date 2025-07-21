using Domain.Dtos.Boards;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.Queries.Boards;

public record GetArchivedBoardsForProcessingQuery(DateTime? ProcessedBefore = null) : IRequest<IEnumerable<BoardDto>>;
