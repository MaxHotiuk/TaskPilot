using MediatR;
using System;

namespace Application.Commands.Boards;

public record MarkBoardAsArchivedCommand(Guid BoardId, string? ArchivalReason = null) : IRequest<bool>;
