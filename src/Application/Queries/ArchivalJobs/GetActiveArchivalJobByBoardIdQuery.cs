using Application.Common.Dtos.Boards;
using MediatR;
using System;

namespace Application.Queries.ArchivalJobs;

public record GetActiveArchivalJobByBoardIdQuery(Guid BoardId) : IRequest<ArchivalJobDto?>;
