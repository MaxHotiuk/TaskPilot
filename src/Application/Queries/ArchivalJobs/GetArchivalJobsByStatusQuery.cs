using Application.Common.Dtos.Boards;
using MediatR;
using System.Collections.Generic;

namespace Application.Queries.ArchivalJobs;

public record GetArchivalJobsByStatusQuery(int Status) : IRequest<IEnumerable<ArchivalJobDto>>;
