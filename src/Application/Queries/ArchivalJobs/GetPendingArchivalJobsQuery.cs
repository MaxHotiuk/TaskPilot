using Domain.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.ArchivalJobs;

public record GetPendingArchivalJobsQuery() : IRequest<IEnumerable<ArchivalJobDto>>;
