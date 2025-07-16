using Application.Common.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.Boards;

public record GetArchivedBoardsPendingJobsQuery() : IRequest<IEnumerable<BoardDto>>;
