using MediatR;
using System;

namespace Application.Commands.ArchivalJobs;

public record CompleteArchivalJobCommand(
    Guid JobId,
    string? BlobPath = null,
    string? ProcessedBy = null
) : IRequest<bool>;
