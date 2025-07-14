using Application.Common.Dtos.Boards;
using Domain.Entities;

namespace Application.Abstractions.Archivation;

public interface IArchivalService
{
    Task<ArchivalJobDto> MarkBoardAsArchivedAsync(Guid boardId, string? archivalReason = null, CancellationToken cancellationToken = default);
    Task ProcessArchivedBoardsAsync(CancellationToken cancellationToken = default);
    Task<bool> EnqueueArchivedBoardAsync(Guid boardId, Guid jobId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ArchivalJobDto>> GetPendingArchivalJobsAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateJobStatusAsync(Guid jobId, ArchivalStatus status, string? errorMessage = null, CancellationToken cancellationToken = default);
}