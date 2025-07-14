using MediatR;
using System;

namespace Application.Commands.ArchivalJobs;

public record UpdateArchivalJobStatusCommand(
    Guid JobId,
    int Status,
    string? ErrorMessage = null,
    string? ProcessedBy = null
) : IRequest<bool>;
