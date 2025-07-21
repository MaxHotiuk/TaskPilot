using Domain.Dtos.Boards;
using MediatR;
using System;

namespace Application.Commands.ArchivalJobs;

public record CreateArchivalJobCommand(
    Guid BoardId,
    string JobType = "BoardArchival",
    string? Metadata = null
) : IRequest<ArchivalJobDto>;
