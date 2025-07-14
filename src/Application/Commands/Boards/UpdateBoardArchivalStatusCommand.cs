using MediatR;
using System;

namespace Application.Commands.Boards;

public record UpdateBoardArchivalStatusCommand(Guid BoardId, bool IsArchived, DateTime? ArchivedAt = null, string? ArchivalReason = null) : IRequest<bool>;
