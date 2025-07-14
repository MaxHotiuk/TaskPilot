using MediatR;
using System;

namespace Application.Commands.ArchivalJobs;

public record FailArchivalJobCommand(
    Guid JobId,
    string ErrorMessage,
    string? ProcessedBy = null
) : IRequest<bool>;
