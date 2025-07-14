using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IArchivalJobRepository : IRepository<ArchivalJob, Guid>
{
    Task<IEnumerable<ArchivalJob>> GetPendingJobsAsync(
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ArchivalJob>> GetJobsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ArchivalJob>> GetJobsByStatusAsync(
        ArchivalStatus status,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ArchivalJob>> GetJobsForProcessingAsync(
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default);
    Task<ArchivalJob?> GetActiveJobByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateJobStatusAsync(
        Guid jobId,
        ArchivalStatus status,
        string? errorMessage = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default);
    Task<bool> CompleteJobAsync(
        Guid jobId,
        string? blobPath = null,
        string? processedBy = null,
        CancellationToken cancellationToken = default);
    Task<bool> FailJobAsync(
        Guid jobId,
        string errorMessage,
        string? processedBy = null,
        CancellationToken cancellationToken = default);
    Task<ArchivalJob> CreateJobAsync(
        Guid boardId,
        string jobType = "BoardArchival",
        string? metadata = null,
        CancellationToken cancellationToken = default);
}