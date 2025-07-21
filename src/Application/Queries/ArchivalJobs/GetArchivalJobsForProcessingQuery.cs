using Domain.Dtos.Boards;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.Queries.ArchivalJobs;

public record GetArchivalJobsForProcessingQuery(DateTime? CreatedBefore = null) : IRequest<IEnumerable<ArchivalJobDto>>;
